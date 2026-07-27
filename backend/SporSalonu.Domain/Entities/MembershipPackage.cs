namespace SporSalonu.Domain.Entities;

/// <summary>
/// Üyelik paket tanımları. Veritabanından yönetilir, koda gömülmez.
/// Örnek: 1 Aylık, 3 Aylık, 6 Aylık, 12 Aylık
/// </summary>
public class MembershipPackage
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;          // "1 Aylık Üyelik"
    public int AySayisi { get; set; }                        // 1, 3, 6, 12
    public decimal Fiyat { get; set; }                       // ₺ cinsinden
    public bool IsActive { get; set; } = true;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<MembershipSubscription> Subscriptions { get; set; } = new List<MembershipSubscription>();
}
