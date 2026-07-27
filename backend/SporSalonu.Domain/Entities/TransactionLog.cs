using SporSalonu.Domain.Enums;

namespace SporSalonu.Domain.Entities;

/// <summary>
/// Tüm finansal hareketler bu tabloda tutulur.
/// Müşterinin bakiyesi bu logların SUM'ından hesaplanır.
/// Borclandirma = negatif etki, Odeme = pozitif etki.
/// </summary>
public class TransactionLog
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public int? SubscriptionId { get; set; }   // Hangi aboneliğe bağlı

    public TransactionType Tip { get; set; }
    public decimal Tutar { get; set; }          // Pozitif değer (işlem tipi negatif/pozitif belirler)
    public string? Aciklama { get; set; }
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;

    // Navigation
    public Member Member { get; set; } = null!;
    public MembershipSubscription? Subscription { get; set; }
}
