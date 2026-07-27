using SporSalonu.Application.DTOs.CheckIn;
using SporSalonu.Application.DTOs.Subscription;

namespace SporSalonu.Application.DTOs.Dashboard;

public record DashboardStatsDto(
    int AktifUyeler,
    decimal AylikGelir,
    int BugunkuGirisler
);

public record DashboardSummaryDto(
    DashboardStatsDto Stats,
    List<CheckInResultDto> SonCheckInler,
    List<SubscriptionDetailDto> YaklasanBitisler
);
