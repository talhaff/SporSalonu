using SporSalonu.Domain.Enums;

namespace SporSalonu.Application.DTOs.Finance;

public record CreatePaymentDto(
    int MemberId,
    int SubscriptionId,
    decimal Tutar,
    string? Aciklama
);

public record TransactionLogDto(
    int Id,
    string IslemTipi,
    decimal Tutar,
    string? Aciklama,
    DateTime IslemTarihi
);

public record MemberFinanceSummaryDto(
    int MemberId,
    string UyeAdSoyad,
    decimal ToplamBorc,
    decimal ToplamOdeme,
    decimal KalanBakiye,
    List<TransactionLogDto> Hareketler
);

public record GlobalTransactionLogDto(
    int Id,
    int MemberId,
    string UyeAdSoyad,
    string IslemTipi,
    decimal Tutar,
    string? Aciklama,
    DateTime IslemTarihi
);

public record GlobalFinanceSummaryDto(
    decimal ToplamTahsilat,
    decimal BekleyenAlacaklar,
    List<GlobalTransactionLogDto> SonIslemler
);
