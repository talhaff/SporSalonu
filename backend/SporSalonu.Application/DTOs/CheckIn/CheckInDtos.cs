namespace SporSalonu.Application.DTOs.CheckIn;

public record CheckInRequestDto(int MemberId, bool IsEntry = true);

public record CheckInResultDto(
    bool IzinVerildi,
    string Mesaj,
    string? UyeAdSoyad,
    DateTime? UyelikBitisi,
    DateTime? GirisTarihi = null,
    DateTime? CikisTarihi = null
);

public record CheckInHistoryDto(
    int Id,
    string UyeAdSoyad,
    string Telefon,
    DateTime GirisTarihi,
    DateTime? CikisTarihi,
    bool IzinVerildi,
    string? RedSebebi
);
