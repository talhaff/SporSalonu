using SporSalonu.Domain.Enums;

namespace SporSalonu.Domain.Entities;

/// <summary>
/// Sistem bildirimleri. Hangfire cron job'ları tarafından üretilir.
/// </summary>
public class SystemNotification
{
    public int Id { get; set; }
    public int? MemberId { get; set; }       // Hangi müşteriyle ilgili (varsa)
    public NotificationType Tip { get; set; }
    public string Mesaj { get; set; } = string.Empty;
    public bool Okundu { get; set; } = false;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    // Navigation
    public Member? Member { get; set; }
}
