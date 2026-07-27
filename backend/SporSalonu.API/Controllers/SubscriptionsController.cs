using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporSalonu.Application.DTOs.Subscription;
using SporSalonu.Application.Interfaces;

namespace SporSalonu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subService;

    public SubscriptionsController(ISubscriptionService subService)
    {
        _subService = subService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sub = await _subService.GetByIdAsync(id);
        if (sub is null) return NotFound(new { basarili = false, mesaj = "Abonelik bulunamadı." });
        return Ok(new { basarili = true, data = sub });
    }

    [HttpGet("uye/{memberId}")]
    public async Task<IActionResult> GetByMember(int memberId) =>
        Ok(new { basarili = true, data = await _subService.GetByMemberIdAsync(memberId) });

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionDto dto)
    {
        var id = await _subService.CreateSubscriptionAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { basarili = true, data = new { id } });
    }

    [HttpPost("dondur")]
    public async Task<IActionResult> Freeze([FromBody] FreezeSubscriptionDto dto)
    {
        await _subService.FreezeAsync(dto);
        return Ok(new { basarili = true, mesaj = "Üyelik donduruldu." });
    }
}
