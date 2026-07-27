using Microsoft.EntityFrameworkCore;
using SporSalonu.Application.DTOs.Dashboard;
using SporSalonu.Application.Interfaces;
using SporSalonu.Infrastructure.Persistence;
using SporSalonu.Domain.Enums;

namespace SporSalonu.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;
    private readonly ICheckInService _checkInService;
    private readonly ISubscriptionService _subscriptionService;

    public DashboardService(
        AppDbContext db,
        ICheckInService checkInService,
        ISubscriptionService subscriptionService)
    {
        _db = db;
        _checkInService = checkInService;
        _subscriptionService = subscriptionService;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var aktifUyeler = await _db.Members.CountAsync(m => m.IsActive);
        
        var buAyBasi = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var aylikGelir = await _db.TransactionLogs
            .Where(t => t.Tip == TransactionType.Odeme && t.IslemTarihi >= buAyBasi)
            .SumAsync(t => t.Tutar);

        var bugunBasi = DateTime.UtcNow.Date;
        var bugunkuGirisler = await _db.CheckInLogs
            .CountAsync(c => c.GirisTarihi >= bugunBasi);

        var stats = new DashboardStatsDto(aktifUyeler, aylikGelir, bugunkuGirisler);

        var sonCheckInler = await _checkInService.GetTodayCheckInsAsync();
        var yaklasanBitisler = await _subscriptionService.GetExpiringSubscriptionsAsync();

        return new DashboardSummaryDto(stats, sonCheckInler, yaklasanBitisler);
    }
}
