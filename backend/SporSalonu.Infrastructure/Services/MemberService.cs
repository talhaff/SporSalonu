using Microsoft.EntityFrameworkCore;
using SporSalonu.Application.DTOs.Member;
using SporSalonu.Application.Interfaces;
using SporSalonu.Domain.Entities;
using SporSalonu.Domain.Enums;
using SporSalonu.Infrastructure.Persistence;

namespace SporSalonu.Infrastructure.Services;

public class MemberService : IMemberService
{
    private readonly AppDbContext _db;

    public MemberService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<MemberListDto>> GetAllAsync()
    {
        var members = await _db.Members
            .Include(m => m.Subscriptions)
                .ThenInclude(s => s.Package)
            .Include(m => m.TransactionLogs)
            .AsNoTracking()
            .ToListAsync();

        return members.Select(m => MapToListDto(m)).ToList();
    }

    public async Task<MemberDetailDto?> GetByIdAsync(int id)
    {
        var m = await _db.Members
            .Include(m => m.Subscriptions)
                .ThenInclude(s => s.Package)
            .Include(m => m.Subscriptions)
                .ThenInclude(s => s.FreezeLogs)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (m is null) return null;

        return new MemberDetailDto(
            m.Id,
            m.Ad,
            m.Soyad,
            m.Telefon,
            m.Email,
            m.KayitTarihi,
            m.IsActive,
            m.CardUid,
            m.Subscriptions.Select(s => new SubscriptionSummaryDto(
                s.Id,
                s.Package?.Ad ?? "-",
                s.BaslangicTarihi,
                s.BitisTarihi,
                s.Durum.ToString(),
                s.NetTutar,
                s.KalanBakiye
            )).ToList()
        );
    }

    public async Task<int> CreateAsync(CreateMemberDto dto)
    {
        var member = new Member
        {
            Ad = dto.Ad.Trim(),
            Soyad = dto.Soyad.Trim(),
            Telefon = dto.Telefon.Trim(),
            Email = dto.Email?.Trim(),
            CardUid = string.IsNullOrWhiteSpace(dto.CardUid) ? null : dto.CardUid.Trim(),
            KayitTarihi = DateTime.UtcNow
        };
        _db.Members.Add(member);
        await _db.SaveChangesAsync();
        return member.Id;
    }

    public async Task UpdateAsync(int id, UpdateMemberDto dto)
    {
        var member = await _db.Members.FindAsync(id)
            ?? throw new KeyNotFoundException($"Üye bulunamadı: {id}");

        member.Ad = dto.Ad.Trim();
        member.Soyad = dto.Soyad.Trim();
        member.Telefon = dto.Telefon.Trim();
        member.Email = dto.Email?.Trim();
        member.CardUid = string.IsNullOrWhiteSpace(dto.CardUid) ? null : dto.CardUid.Trim();

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var member = await _db.Members.FindAsync(id)
            ?? throw new KeyNotFoundException($"Üye bulunamadı: {id}");

        member.IsActive = false;   // Soft delete
        await _db.SaveChangesAsync();
    }

    public async Task<MemberListDto?> GetByPhoneAsync(string telefon)
    {
        var m = await _db.Members
            .Include(m => m.Subscriptions).ThenInclude(s => s.Package)
            .Include(m => m.TransactionLogs)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Telefon == telefon.Trim());

        return m is null ? null : MapToListDto(m);
    }

    public async Task<MemberListDto?> GetByCardUidAsync(string cardUid)
    {
        var m = await _db.Members
            .Include(m => m.Subscriptions).ThenInclude(s => s.Package)
            .Include(m => m.TransactionLogs)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.CardUid == cardUid.Trim());

        return m is null ? null : MapToListDto(m);
    }

    // ── Private Helpers ────────────────────────────────────────
    private static MemberListDto MapToListDto(Member m)
    {
        var aktifSub = m.Subscriptions
            .FirstOrDefault(s => s.Durum == SubscriptionStatus.Aktif);

        // Kalan bakiye: Borçlandırma logları - Ödeme logları
        var kalanBakiye = m.TransactionLogs
            .Where(t => t.SubscriptionId == aktifSub?.Id)
            .Sum(t => t.Tip == TransactionType.Borclandirma ? t.Tutar
                    : t.Tip == TransactionType.Odeme ? -t.Tutar
                    : 0m);

        return new MemberListDto(
            m.Id,
            $"{m.Ad} {m.Soyad}",
            m.Telefon,
            m.Email,
            m.KayitTarihi,
            m.IsActive,
            aktifSub?.Package?.Ad,
            aktifSub?.BitisTarihi,
            kalanBakiye,
            m.CardUid
        );
    }
}
