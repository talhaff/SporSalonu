using Microsoft.EntityFrameworkCore;
using SporSalonu.Application.DTOs.Subscription;
using SporSalonu.Application.Interfaces;
using SporSalonu.Domain.Entities;
using SporSalonu.Domain.Enums;
using SporSalonu.Infrastructure.Persistence;

namespace SporSalonu.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _db;

    public SubscriptionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<SubscriptionDetailDto?> GetByIdAsync(int id)
    {
        var s = await _db.MembershipSubscriptions
            .Include(s => s.Member)
            .Include(s => s.Package)
            .Include(s => s.FreezeLogs)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        return s is null ? null : MapToDetailDto(s);
    }

    public async Task<List<SubscriptionDetailDto>> GetByMemberIdAsync(int memberId)
    {
        var subs = await _db.MembershipSubscriptions
            .Include(s => s.Member)
            .Include(s => s.Package)
            .Include(s => s.FreezeLogs)
            .AsNoTracking()
            .Where(s => s.MemberId == memberId)
            .OrderByDescending(s => s.OlusturmaTarihi)
            .ToListAsync();

        return subs.Select(MapToDetailDto).ToList();
    }

    public async Task<List<SubscriptionDetailDto>> GetExpiringSubscriptionsAsync()
    {
        var sinirTarih = DateTime.UtcNow.AddDays(7);
        
        var subs = await _db.MembershipSubscriptions
            .Include(s => s.Member)
            .Include(s => s.Package)
            .Include(s => s.FreezeLogs)
            .AsNoTracking()
            .Where(s => s.Durum == SubscriptionStatus.Aktif && s.BitisTarihi <= sinirTarih)
            .OrderBy(s => s.BitisTarihi)
            .Take(10)
            .ToListAsync();

        return subs.Select(MapToDetailDto).ToList();
    }

    /// <summary>
    /// Borçlandırma Algoritması:
    /// 1. Paketin fiyatını çek
    /// 2. NetTutar = Fiyat - IndirimTutari
    /// 3. Aboneliği kaydet
    /// 4. TransactionLog: Borclandirma (NetTutar)
    /// 5. TransactionLog: Odeme (PesinatTutari)
    /// 6. KalanBakiye = NetTutar - PesinatTutari
    /// </summary>
    public async Task<int> CreateSubscriptionAsync(CreateSubscriptionDto dto)
    {
        var paket = await _db.MembershipPackages.FindAsync(dto.PackageId)
            ?? throw new KeyNotFoundException($"Paket bulunamadı: {dto.PackageId}");

        var netTutar = paket.Fiyat - dto.IndirimTutari;
        if (netTutar < 0) throw new InvalidOperationException("İndirim tutarı paket fiyatından büyük olamaz.");

        var bitisTarihi = paket.GunSayisi > 0
            ? dto.BaslangicTarihi.AddDays(paket.GunSayisi)
            : dto.BaslangicTarihi.AddMonths(paket.AySayisi);
        var kalanBakiye = netTutar - dto.PesinatTutari;

        var subscription = new MembershipSubscription
        {
            MemberId = dto.MemberId,
            MembershipPackageId = dto.PackageId,
            BaslangicTarihi = dto.BaslangicTarihi,
            BitisTarihi = bitisTarihi,
            ToplamTutar = paket.Fiyat,
            IndirimTutari = dto.IndirimTutari,
            NetTutar = netTutar,
            KalanBakiye = kalanBakiye,
            Durum = SubscriptionStatus.Aktif,
            OlusturmaTarihi = DateTime.UtcNow
        };

        _db.MembershipSubscriptions.Add(subscription);
        await _db.SaveChangesAsync();

        // Borçlandırma log'u
        _db.TransactionLogs.Add(new TransactionLog
        {
            MemberId = dto.MemberId,
            SubscriptionId = subscription.Id,
            Tip = TransactionType.Borclandirma,
            Tutar = netTutar,
            Aciklama = $"{paket.Ad} paketi borçlandırması{(dto.IndirimTutari > 0 ? $" ({dto.IndirimTutari:C} indirimli)" : "")}",
            IslemTarihi = DateTime.UtcNow
        });

        // Peşinat ödeme log'u
        if (dto.PesinatTutari > 0)
        {
            _db.TransactionLogs.Add(new TransactionLog
            {
                MemberId = dto.MemberId,
                SubscriptionId = subscription.Id,
                Tip = TransactionType.Odeme,
                Tutar = dto.PesinatTutari,
                Aciklama = dto.Aciklama ?? "Peşinat ödemesi",
                IslemTarihi = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
        return subscription.Id;
    }

    /// <summary>
    /// Dondurma Algoritması:
    /// 1. Mevcut BitisTarihine + GunSayisi ekle
    /// 2. FreezeLog oluştur
    /// 3. Durum = Dondurulmus
    /// </summary>
    public async Task FreezeAsync(FreezeSubscriptionDto dto)
    {
        var sub = await _db.MembershipSubscriptions.FindAsync(dto.SubscriptionId)
            ?? throw new KeyNotFoundException($"Abonelik bulunamadı: {dto.SubscriptionId}");

        if (sub.Durum != SubscriptionStatus.Aktif)
            throw new InvalidOperationException("Sadece aktif üyelikler dondurulabilir.");

        var yeniBitisTarihi = sub.BitisTarihi.AddDays(dto.GunSayisi);

        var freezeLog = new FreezeLog
        {
            SubscriptionId = sub.Id,
            DondurmaBaslangici = dto.DondurmaBaslangici,
            DondurulanGunSayisi = dto.GunSayisi,
            YeniBitisTarihi = yeniBitisTarihi,
            Sebep = dto.Sebep,
            OlusturmaTarihi = DateTime.UtcNow
        };

        sub.BitisTarihi = yeniBitisTarihi;
        sub.Durum = SubscriptionStatus.Dondurulmus;

        _db.FreezeLogs.Add(freezeLog);
        await _db.SaveChangesAsync();
    }

    /// <summary>Hangfire Job: Süresi dolmuş aktif üyelikleri pasife çeker.</summary>
    public async Task ProcessExpiredSubscriptionsAsync()
    {
        var simdi = DateTime.UtcNow;
        var surenDolmuslar = await _db.MembershipSubscriptions
            .Where(s => s.Durum == SubscriptionStatus.Aktif && s.BitisTarihi < simdi)
            .ToListAsync();

        foreach (var s in surenDolmuslar)
        {
            s.Durum = SubscriptionStatus.Suresi_Dolmus;

            _db.SystemNotifications.Add(new SystemNotification
            {
                MemberId = s.MemberId,
                Tip = NotificationType.UyelikSuresiDoldu,
                Mesaj = $"Üyelik süresi {s.BitisTarihi:dd.MM.yyyy} tarihinde doldu. Abonelik pasife alındı.",
                OlusturmaTarihi = DateTime.UtcNow
            });
        }

        if (surenDolmuslar.Any())
            await _db.SaveChangesAsync();
    }

    /// <summary>Hangfire Job: Bitişine 3 gün kalan üyelikler için bildirim oluşturur.</summary>
    public async Task SendExpiryNotificationsAsync()
    {
        var hedefTarih = DateTime.UtcNow.AddDays(3).Date;
        var yaklasanlar = await _db.MembershipSubscriptions
            .Include(s => s.Member)
            .Where(s => s.Durum == SubscriptionStatus.Aktif
                     && s.BitisTarihi.Date == hedefTarih)
            .ToListAsync();

        foreach (var s in yaklasanlar)
        {
            // Aynı gün için mükerrer bildirim oluşturma
            var mevcutVar = await _db.SystemNotifications.AnyAsync(
                n => n.MemberId == s.MemberId
                  && n.Tip == NotificationType.UyelikBitisUyarisi
                  && n.OlusturmaTarihi.Date == DateTime.UtcNow.Date);

            if (!mevcutVar)
            {
                _db.SystemNotifications.Add(new SystemNotification
                {
                    MemberId = s.MemberId,
                    Tip = NotificationType.UyelikBitisUyarisi,
                    Mesaj = $"{s.Member.Ad} {s.Member.Soyad} adlı üyenin üyeliği {s.BitisTarihi:dd.MM.yyyy} tarihinde bitiyor (3 gün kaldı).",
                    OlusturmaTarihi = DateTime.UtcNow
                });
            }
        }

        if (yaklasanlar.Any())
            await _db.SaveChangesAsync();
    }

    // ── Private Helpers ─────────────────────────────────────────
    private static SubscriptionDetailDto MapToDetailDto(MembershipSubscription s) =>
        new(
            s.Id,
            s.MemberId,
            s.Member is not null ? $"{s.Member.Ad} {s.Member.Soyad}" : "-",
            s.Package?.Ad ?? "-",
            s.BaslangicTarihi,
            s.BitisTarihi,
            s.ToplamTutar,
            s.IndirimTutari,
            s.NetTutar,
            s.KalanBakiye,
            s.Durum.ToString(),
            s.FreezeLogs.Select(f => new FreezeLogDto(
                f.Id, f.DondurmaBaslangici, f.DondurulanGunSayisi,
                f.YeniBitisTarihi, f.Sebep, f.OlusturmaTarihi
            )).ToList()
        );
}
