using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Infrastructure.Persistence;

namespace SporSalonu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PackagesController : ControllerBase
{
    private readonly AppDbContext _db;

    public PackagesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var paketler = await _db.MembershipPackages
            .Where(p => p.IsActive)
            .OrderBy(p => p.AySayisi)
            .AsNoTracking()
            .ToListAsync();

        return Ok(new { basarili = true, data = paketler });
    }
}
