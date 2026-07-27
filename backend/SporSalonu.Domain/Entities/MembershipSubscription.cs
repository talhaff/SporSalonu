using SporSalonu.Domain.Enums;

namespace SporSalonu.Domain.Entities;

/// <summary>
/// Müşteri-Paket ilişkisi. Bir müşterinin aktif veya geçmiş aboneliklerini tutar.
/// </summary>
public class MembershipSubscription
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public int MembershipPackageId { get; set; }

    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }

    /// <summary>Paketin toplam fiyatı (indirim öncesi).</summary>
    public decimal ToplamTutar { get; set; }

    /// <summary>Uygulanan indirim tutarı.</summary>
    public decimal IndirimTutari { get; set; }

    /// <summary>Net ödenecek tutar = ToplamTutar - IndirimTutari</summary>
    public decimal NetTutar { get; set; }

    /// <summary>Kalan bakiye servis katmanında TransactionLogs'tan hesaplanır.</summary>
    public decimal KalanBakiye { get; set; }

    public SubscriptionStatus Durum { get; set; } = SubscriptionStatus.Aktif;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    // Navigation
    public Member Member { get; set; } = null!;
    public MembershipPackage Package { get; set; } = null!;
    public ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();
    public ICollection<FreezeLog> FreezeLogs { get; set; } = new List<FreezeLog>();
}
