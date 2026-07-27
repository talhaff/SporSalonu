namespace SporSalonu.Application.DTOs.CheckIn;

public record CheckInRequestDto(int MemberId);

public record CheckInResultDto(
    bool IzinVerildi,
    string Mesaj,
    string? UyeAdSoyad,
    DateTime? UyelikBitisi
);
