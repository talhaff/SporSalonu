namespace SporSalonu.Domain.Entities;

/// <summary>
/// Üyelik dondurma kayıtları.
/// Dondurma işlemi: BitisTarihi += DondurulanGunSayisi
/// </summary>
public class FreezeLog
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }

    public DateTime DondurmaBaslangici { get; set; }
    public int DondurulanGunSayisi { get; set; }
    public DateTime YeniBitisTarihi { get; set; }
    public string? Sebep { get; set; }   // "Hastalık", "Seyahat" vb.
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    // Navigation
    public MembershipSubscription Subscription { get; set; } = null!;
}
