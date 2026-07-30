using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporSalonu.Application.DTOs.CheckIn;
using SporSalonu.Application.Interfaces;

namespace SporSalonu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckInController : ControllerBase
{
    private readonly ICheckInService _checkInService;

    public CheckInController(ICheckInService checkInService)
    {
        _checkInService = checkInService;
    }

    /// <summary>Üye giriş kontrolü — ana iş kuralı endpoint'i.</summary>
    [HttpPost]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto dto)
    {
        var result = await _checkInService.ProcessCheckInAsync(dto);
        // HTTP 200 dön ama body'de izinVerildi:false olabilir — UI bunu işler
        return Ok(new { basarili = true, data = result });
    }

    /// <summary>Bugünün giriş logları.</summary>
    [HttpGet("bugun")]
    public async Task<IActionResult> TodayCheckIns() =>
        Ok(new { basarili = true, data = await _checkInService.GetTodayCheckInsAsync() });

    /// <summary>Tüm geçiş (Check-in / Check-out) geçmişi.</summary>
    [HttpGet("gecmis")]
    public async Task<IActionResult> CheckInHistory() =>
        Ok(new { basarili = true, data = await _checkInService.GetCheckInHistoryAsync() });
}
