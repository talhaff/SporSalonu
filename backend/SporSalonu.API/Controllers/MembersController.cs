using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporSalonu.Application.DTOs.Member;
using SporSalonu.Application.Interfaces;

namespace SporSalonu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(new { basarili = true, data = await _memberService.GetAllAsync() });

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member is null) return NotFound(new { basarili = false, mesaj = "Üye bulunamadı." });
        return Ok(new { basarili = true, data = member });
    }

    [HttpGet("telefon/{telefon}")]
    public async Task<IActionResult> GetByPhone(string telefon)
    {
        var member = await _memberService.GetByPhoneAsync(telefon);
        if (member is null) return NotFound(new { basarili = false, mesaj = "Bu telefon numarasıyla kayıtlı üye bulunamadı." });
        return Ok(new { basarili = true, data = member });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMemberDto dto)
    {
        var id = await _memberService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { basarili = true, data = new { id } });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMemberDto dto)
    {
        await _memberService.UpdateAsync(id, dto);
        return Ok(new { basarili = true, mesaj = "Üye güncellendi." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _memberService.DeleteAsync(id);
        return Ok(new { basarili = true, mesaj = "Üye pasife alındı." });
    }
}
