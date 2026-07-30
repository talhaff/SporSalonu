using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SporSalonu.API.Hubs;
using SporSalonu.Application.DTOs.CheckIn;
using SporSalonu.Application.Interfaces;
using SporSalonu.Domain.Interfaces;

namespace SporSalonu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HardwareController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly ICheckInService _checkInService;
    private readonly ITurnstileService _turnstileService;
    private readonly IHubContext<CheckInHub> _hubContext;

    public HardwareController(
        IMemberService memberService,
        ICheckInService checkInService,
        ITurnstileService turnstileService,
        IHubContext<CheckInHub> hubContext)
    {
        _memberService = memberService;
        _checkInService = checkInService;
        _turnstileService = turnstileService;
        _hubContext = hubContext;
    }

    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn([FromBody] HardwareCheckInRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CardUid))
        {
            return BadRequest(new { Message = "Kart UID boş olamaz." });
        }

        var member = await _memberService.GetByCardUidAsync(request.CardUid);
        if (member == null)
        {
            return BadRequest(new { Message = "Bu karta sahip bir üye bulunamadı." });
        }

        // ProcessCheckInAsync memberId alır ve gerekli iş kurallarını işletir
        var result = await _checkInService.ProcessCheckInAsync(new CheckInRequestDto(member.Id));

        if (result.IzinVerildi)
        {
            await _turnstileService.OpenGateAsync(isEntry: true);
        }

        // SignalR ile anlık bildirim fırlat
        await _hubContext.Clients.All.SendAsync("OnCheckIn", result);

        if (result.IzinVerildi)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }

    [HttpPost("manual-override")]
    public async Task<IActionResult> ManualOverride([FromBody] ManualOverrideRequest request)
    {
        var success = await _turnstileService.OpenGateAsync(isEntry: request.IsEntry);
        if (success)
        {
            var direction = request.IsEntry ? "Giriş" : "Çıkış";
            // İsteğe bağlı olarak manual override loglanabilir
            return Ok(new { Message = $"Turnike {direction} kapısı manuel olarak açıldı." });
        }

        return StatusCode(500, new { Message = "Donanım servisiyle iletişim kurulamadı." });
    }
}

public class HardwareCheckInRequest
{
    public string CardUid { get; set; } = string.Empty;
}

public class ManualOverrideRequest
{
    public bool IsEntry { get; set; } = true;
}
