using Microsoft.EntityFrameworkCore;
using SporSalonu.Application.DTOs.Finance;
using SporSalonu.Application.Interfaces;
using SporSalonu.Domain.Entities;
using SporSalonu.Domain.Enums;
using SporSalonu.Infrastructure.Persistence;

namespace SporSalonu.Infrastructure.Services;

public class FinanceService : IFinanceService
{
    private readonly AppDbContext _db;

    public FinanceService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<MemberFinanceSummaryDto> GetMemberFinanceSummaryAsync(int memberId)
    {
        var member = await _db.Members.FindAsync(memberId)
            ?? throw new KeyNotFoundException($"Üye bulunamadı: {memberId}");

        var logs = await _db.TransactionLogs
            .Where(t => t.MemberId == memberId)
            .OrderByDescending(t => t.IslemTarihi)
            .AsNoTracking()
            .ToListAsync();

        var toplamBorc = logs.Where(t => t.Tip == TransactionType.Borclandirma).Sum(t => t.Tutar);
        var toplamOdeme = logs.Where(t => t.Tip == TransactionType.Odeme).Sum(t => t.Tutar);
        var kalanBakiye = toplamBorc - toplamOdeme;

        return new MemberFinanceSummaryDto(
            memberId,
            $"{member.Ad} {member.Soyad}",
            toplamBorc,
            toplamOdeme,
            kalanBakiye,
            logs.Select(l => new TransactionLogDto(
                l.Id,
                l.Tip.ToString(),
                l.Tutar,
                l.Aciklama,
                l.IslemTarihi
            )).ToList()
        );
    }

    public async Task RecordPaymentAsync(CreatePaymentDto dto)
    {
        if (dto.Tutar <= 0)
            throw new ArgumentException("Ödeme tutarı 0'dan büyük olmalıdır.");

        var sub = await _db.MembershipSubscriptions.FindAsync(dto.SubscriptionId)
            ?? throw new KeyNotFoundException($"Abonelik bulunamadı: {dto.SubscriptionId}");

        // Ödeme log'u ekle
        _db.TransactionLogs.Add(new TransactionLog
        {
            MemberId = dto.MemberId,
            SubscriptionId = dto.SubscriptionId,
            Tip = TransactionType.Odeme,
            Tutar = dto.Tutar,
            Aciklama = dto.Aciklama ?? "Manuel ödeme",
            IslemTarihi = DateTime.UtcNow
        });

        // Aboneliğin kalan bakiyesini güncelle
        sub.KalanBakiye = Math.Max(0, sub.KalanBakiye - dto.Tutar);

        await _db.SaveChangesAsync();
    }

    public async Task<GlobalFinanceSummaryDto> GetGlobalFinanceSummaryAsync()
    {
        var logs = await _db.TransactionLogs
            .Include(t => t.Member)
            .OrderByDescending(t => t.IslemTarihi)
            .Take(100) // Son 100 işlem
            .AsNoTracking()
            .ToListAsync();

        var buAyBasi = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        
        var toplamTahsilat = await _db.TransactionLogs
            .Where(t => t.Tip == TransactionType.Odeme && t.IslemTarihi >= buAyBasi)
            .SumAsync(t => t.Tutar);

        var bekleyenAlacaklar = await _db.Members
            .Where(m => m.IsActive)
            .SumAsync(m => m.Subscriptions.Where(s => s.Durum == SubscriptionStatus.Aktif).Sum(s => s.KalanBakiye));

        return new GlobalFinanceSummaryDto(
            toplamTahsilat,
            bekleyenAlacaklar,
            logs.Select(l => new GlobalTransactionLogDto(
                l.Id,
                l.MemberId,
                l.Member != null ? $"{l.Member.Ad} {l.Member.Soyad}" : "Bilinmeyen Üye",
                l.Tip.ToString(),
                l.Tutar,
                l.Aciklama,
                l.IslemTarihi
            )).ToList()
        );
    }
}
