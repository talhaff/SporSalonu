namespace SporSalonu.Application.DTOs.Auth;

public record LoginRequestDto(string KullaniciAdi, string Sifre);

public record LoginResponseDto(string KullaniciAdi, string Rol, DateTime ExpireAt);

public record RegisterUserDto(
    string KullaniciAdi,
    string Email,
    string Sifre,
    string Rol = "Personel"
);
