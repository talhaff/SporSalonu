using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Infrastructure.Persistence;

namespace SporSalonu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public NotificationsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetUnread()
    {
        var bildirimler = await _db.SystemNotifications
            .Include(n => n.Member)
            .Where(n => !n.Okundu)
            .OrderByDescending(n => n.OlusturmaTarihi)
            .Take(50)
            .AsNoTracking()
            .ToListAsync();

        return Ok(new { basarili = true, data = bildirimler });
    }

    [HttpPut("{id}/okundu")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var bildirim = await _db.SystemNotifications.FindAsync(id);
        if (bildirim is null) return NotFound();

        bildirim.Okundu = true;
        await _db.SaveChangesAsync();
        return Ok(new { basarili = true });
    }

    [HttpPut("tumunu-okundu")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        await _db.SystemNotifications
            .Where(n => !n.Okundu)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.Okundu, true));

        return Ok(new { basarili = true, mesaj = "Tüm bildirimler okundu." });
    }
}
