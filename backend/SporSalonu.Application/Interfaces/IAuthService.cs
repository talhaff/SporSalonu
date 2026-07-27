using SporSalonu.Application.DTOs.Auth;

namespace SporSalonu.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
    Task RegisterAsync(RegisterUserDto dto);
}
