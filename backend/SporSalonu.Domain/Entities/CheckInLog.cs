namespace SporSalonu.Domain.Entities;

/// <summary>
/// Müşteri giriş logları. Her giriş denemesi (izin verilsin ya da verilmesin) kaydedilir.
/// </summary>
public class CheckInLog
{
    public int Id { get; set; }
    public int MemberId { get; set; }

    public DateTime GirisTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? CikisTarihi { get; set; }
    public bool IzinVerildi { get; set; }
    public string? RedSebebi { get; set; }   // "Üyelik süresi dolmuş", "Borç var", "Dondurulmuş" vb.

    // Navigation
    public Member Member { get; set; } = null!;
}
