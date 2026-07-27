namespace SporSalonu.Application.DTOs.Subscription;

public record CreateSubscriptionDto(
    int MemberId,
    int PackageId,
    DateTime BaslangicTarihi,
    decimal IndirimTutari,
    decimal PesinatTutari,
    string? Aciklama
);

public record FreezeSubscriptionDto(
    int SubscriptionId,
    DateTime DondurmaBaslangici,
    int GunSayisi,
    string? Sebep
);

public record SubscriptionDetailDto(
    int Id,
    int MemberId,
    string UyeAdSoyad,
    string PaketAdi,
    DateTime BaslangicTarihi,
    DateTime BitisTarihi,
    decimal ToplamTutar,
    decimal IndirimTutari,
    decimal NetTutar,
    decimal KalanBakiye,
    string Durum,
    List<FreezeLogDto> FreezeLogs
);

public record FreezeLogDto(
    int Id,
    DateTime DondurmaBaslangici,
    int DondurulanGunSayisi,
    DateTime YeniBitisTarihi,
    string? Sebep,
    DateTime OlusturmaTarihi
);
