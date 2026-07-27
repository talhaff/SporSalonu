namespace SporSalonu.Domain.Entities;

/// <summary>
/// Müşteri (Üye) ana entity'si.
/// </summary>
public class Member
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ICollection<MembershipSubscription> Subscriptions { get; set; } = new List<MembershipSubscription>();
    public ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();
    public ICollection<CheckInLog> CheckInLogs { get; set; } = new List<CheckInLog>();
}
