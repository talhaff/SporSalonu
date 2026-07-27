using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporSalonu.Application.Interfaces;

namespace SporSalonu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("ozet")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _dashboardService.GetDashboardSummaryAsync();
        return Ok(new { basarili = true, data = result });
    }
}
