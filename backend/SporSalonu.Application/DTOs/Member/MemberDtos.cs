namespace SporSalonu.Application.DTOs.Member;

public record CreateMemberDto(
    string Ad,
    string Soyad,
    string Telefon,
    string? Email
);

public record UpdateMemberDto(
    string Ad,
    string Soyad,
    string Telefon,
    string? Email
);

public record MemberListDto(
    int Id,
    string AdSoyad,
    string Telefon,
    string? Email,
    DateTime KayitTarihi,
    bool IsActive,
    string? AktifPaket,
    DateTime? UyelikBitisi,
    decimal KalanBakiye
);

public record MemberDetailDto(
    int Id,
    string Ad,
    string Soyad,
    string Telefon,
    string? Email,
    DateTime KayitTarihi,
    bool IsActive,
    List<SubscriptionSummaryDto> Subscriptions
);

public record SubscriptionSummaryDto(
    int Id,
    string PaketAdi,
    DateTime BaslangicTarihi,
    DateTime BitisTarihi,
    string Durum,
    decimal NetTutar,
    decimal KalanBakiye
);
