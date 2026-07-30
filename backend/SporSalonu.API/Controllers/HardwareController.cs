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

        // ProcessCheckInAsync memberId ve IsEntry alır ve gerekli iş kurallarını işletir
        var result = await _checkInService.ProcessCheckInAsync(new CheckInRequestDto(member.Id, request.IsEntry));

        if (result.IzinVerildi)
        {
            await _turnstileService.OpenGateAsync(isEntry: request.IsEntry);
        }

        // SignalR ile anlık bildirim fırlat
        await _hubContext.Clients.All.SendAsync("OnCheckIn", result);

        // Donanıma her halükarda 200 OK dönüyoruz, donanım `IzinVerildi` değerine bakarak kapıyı açıp açmayacağına karar verir.
        return Ok(result);
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

    [HttpGet("debug")]
    public async Task<IActionResult> DebugMembers()
    {
        var members = await _memberService.GetAllAsync();
        return Ok(members.Select(m => new { m.AdSoyad, m.CardUid }));
    }
}

public class HardwareCheckInRequest
{
    public string CardUid { get; set; } = string.Empty;
    public bool IsEntry { get; set; } = true;
}

public class ManualOverrideRequest
{
    public bool IsEntry { get; set; } = true;
}
