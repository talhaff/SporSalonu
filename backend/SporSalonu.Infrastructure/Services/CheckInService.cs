using Microsoft.EntityFrameworkCore;
using SporSalonu.Application.DTOs.CheckIn;
using SporSalonu.Application.Interfaces;
using SporSalonu.Domain.Entities;
using SporSalonu.Domain.Enums;
using SporSalonu.Infrastructure.Persistence;

namespace SporSalonu.Infrastructure.Services;

public class CheckInService : ICheckInService
{
    private readonly AppDbContext _db;

    public CheckInService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Giriş Kontrol Algoritması:
    /// 1. Aktif üyelik var mı?
    /// 2. Dondurulmuş mu?
    /// 3. Gecikmiş borcu var mı?
    /// </summary>
    public async Task<CheckInResultDto> ProcessCheckInAsync(CheckInRequestDto dto)
    {
        var member = await _db.Members
            .Include(m => m.Subscriptions)
            .Include(m => m.TransactionLogs)
            .FirstOrDefaultAsync(m => m.Id == dto.MemberId && m.IsActive);

        if (member is null)
        {
            return new CheckInResultDto(false, "Üye bulunamadı veya hesap pasif.", null, null);
        }

        var simdi = DateTime.UtcNow;

        // EĞER ÇIKIŞ İŞLEMİ İSE:
        if (!dto.IsEntry)
        {
            var enSonGirisLog = await _db.CheckInLogs
                .Where(c => c.MemberId == member.Id && c.IzinVerildi && c.CikisTarihi == null)
                .OrderByDescending(c => c.GirisTarihi)
                .FirstOrDefaultAsync();

            if (enSonGirisLog != null)
            {
                enSonGirisLog.CikisTarihi = simdi;
                await _db.SaveChangesAsync();
                return new CheckInResultDto(true, $"İyi günler, {member.Ad}! Çıkışınız kaydedildi.", $"{member.Ad} {member.Soyad}", null, enSonGirisLog.GirisTarihi, simdi);
            }
            else
            {
                // Giriş kaydı olmadan doğrudan çıkış okutulduysa yeni log atalım
                var cikisLog = new CheckInLog
                {
                    MemberId = member.Id,
                    GirisTarihi = simdi,
                    CikisTarihi = simdi,
                    IzinVerildi = true,
                    RedSebebi = "Doğrudan Çıkış"
                };
                _db.CheckInLogs.Add(cikisLog);
                await _db.SaveChangesAsync();
                return new CheckInResultDto(true, $"İyi günler, {member.Ad}! Çıkışınız kaydedildi.", $"{member.Ad} {member.Soyad}", null, simdi, simdi);
            }
        }

        // Kural 1: Aktif üyelik kontrolü
        var aktifUyelik = member.Subscriptions
            .FirstOrDefault(s => s.Durum == SubscriptionStatus.Aktif && s.BitisTarihi > simdi);

        if (aktifUyelik is null)
        {
            await KaydetCheckIn(member.Id, false, "Aktif üyelik bulunamadı.");
            return new CheckInResultDto(false, "Aktif üyelik bulunamadı.", $"{member.Ad} {member.Soyad}", null);
        }

        // Kural 2: Dondurulmuş üyelik kontrolü
        if (aktifUyelik.Durum == SubscriptionStatus.Dondurulmus)
        {
            await KaydetCheckIn(member.Id, false, "Üyelik dondurulmuş.");
            return new CheckInResultDto(false, "Üyelik dondurulmuş durumda. Giriş yapılamaz.", $"{member.Ad} {member.Soyad}", aktifUyelik.BitisTarihi);
        }

        // Kural 3: Borç kontrolü (KalanBakiye > 0 ise borç var)
        if (aktifUyelik.KalanBakiye > 0)
        {
            await KaydetCheckIn(member.Id, false, $"Gecikmiş borç: {aktifUyelik.KalanBakiye:C}");
            return new CheckInResultDto(false, $"Gecikmiş borcunuz bulunmaktadır: {aktifUyelik.KalanBakiye:C2}. Lütfen ödeme yapınız.", $"{member.Ad} {member.Soyad}", aktifUyelik.BitisTarihi);
        }

        // Tüm kontroller geçti: GİRİŞ İZNİ
        var yeniLog = await KaydetCheckIn(member.Id, true, null);
        return new CheckInResultDto(true, $"Hoş geldiniz, {member.Ad}! Üyeliğiniz {aktifUyelik.BitisTarihi:dd.MM.yyyy} tarihine kadar geçerlidir.", $"{member.Ad} {member.Soyad}", aktifUyelik.BitisTarihi, yeniLog.GirisTarihi, null);
    }

    public async Task<List<CheckInResultDto>> GetTodayCheckInsAsync()
    {
        var bugun = DateTime.UtcNow.Date;
        var loglar = await _db.CheckInLogs
            .Include(c => c.Member)
            .Where(c => c.GirisTarihi.Date == bugun)
            .OrderByDescending(c => c.GirisTarihi)
            .AsNoTracking()
            .ToListAsync();

        return loglar.Select(l => new CheckInResultDto(
            l.IzinVerildi,
            l.RedSebebi ?? (l.CikisTarihi != null ? "Çıkış Yapıldı" : "Giriş Onaylandı"),
            l.Member != null ? $"{l.Member.Ad} {l.Member.Soyad}" : "-",
            null,
            l.GirisTarihi,
            l.CikisTarihi
        )).ToList();
    }

    public async Task<List<CheckInHistoryDto>> GetCheckInHistoryAsync()
    {
        var loglar = await _db.CheckInLogs
            .Include(c => c.Member)
            .OrderByDescending(c => c.GirisTarihi)
            .Take(100)
            .AsNoTracking()
            .ToListAsync();

        return loglar.Select(l => new CheckInHistoryDto(
            l.Id,
            l.Member != null ? $"{l.Member.Ad} {l.Member.Soyad}" : "Bilinmeyen Üye",
            l.Member?.Telefon ?? "-",
            l.GirisTarihi,
            l.CikisTarihi,
            l.IzinVerildi,
            l.RedSebebi
        )).ToList();
    }

    private async Task<CheckInLog> KaydetCheckIn(int memberId, bool izin, string? sebep)
    {
        var log = new CheckInLog
        {
            MemberId = memberId,
            GirisTarihi = DateTime.UtcNow,
            IzinVerildi = izin,
            RedSebebi = sebep
        };
        _db.CheckInLogs.Add(log);
        await _db.SaveChangesAsync();
        return log;
    }
}
