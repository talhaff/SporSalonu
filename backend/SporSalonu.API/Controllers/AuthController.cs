using Microsoft.AspNetCore.Mvc;
using SporSalonu.Application.DTOs.Auth;
using SporSalonu.Application.Interfaces;
using SporSalonu.Infrastructure.Services;

namespace SporSalonu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly AuthService _jwtService;

    public AuthController(IAuthService authService, AuthService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    [HttpPost("giris")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result is null)
            return Unauthorized(new { basarili = false, mesaj = "Kullanıcı adı veya şifre hatalı." });

        var token = _jwtService.GenerateJwtToken(result.KullaniciAdi, result.Rol);

        // HTTP-Only Cookie olarak set et
        Response.Cookies.Append("jwt_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,          // Prod'da true yapın
            SameSite = SameSiteMode.Lax,
            Expires = result.ExpireAt
        });

        return Ok(new { basarili = true, data = result });
    }

    [HttpPost("cikis")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt_token");
        return Ok(new { basarili = true, mesaj = "Çıkış yapıldı." });
    }

    [HttpPost("kayit")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        await _authService.RegisterAsync(dto);
        return Ok(new { basarili = true, mesaj = "Kullanıcı oluşturuldu." });
    }
}
