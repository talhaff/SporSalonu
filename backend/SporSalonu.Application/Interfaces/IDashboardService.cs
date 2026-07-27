using SporSalonu.Application.DTOs.Dashboard;

namespace SporSalonu.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
}
