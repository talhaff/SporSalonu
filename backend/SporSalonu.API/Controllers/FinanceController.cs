using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporSalonu.Application.DTOs.Finance;
using SporSalonu.Application.Interfaces;

namespace SporSalonu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinanceController : ControllerBase
{
    private readonly IFinanceService _financeService;

    public FinanceController(IFinanceService financeService)
    {
        _financeService = financeService;
    }

    [HttpGet("uye/{memberId}")]
    public async Task<IActionResult> GetSummary(int memberId) =>
        Ok(new { basarili = true, data = await _financeService.GetMemberFinanceSummaryAsync(memberId) });

    [HttpGet("islemler")]
    public async Task<IActionResult> GetGlobalSummary() =>
        Ok(new { basarili = true, data = await _financeService.GetGlobalFinanceSummaryAsync() });

    [HttpPost("odeme")]
    public async Task<IActionResult> RecordPayment([FromBody] CreatePaymentDto dto)
    {
        await _financeService.RecordPaymentAsync(dto);
        return Ok(new { basarili = true, mesaj = "Ödeme kaydedildi." });
    }
}
