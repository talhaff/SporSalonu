namespace SporSalonu.Domain.Entities;

/// <summary>
/// Admin/Personel kullanıcıları. JWT authentication için kullanılır.
/// </summary>
public class AppUser
{
    public int Id { get; set; }
    public string KullaniciAdi { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Personel";   // "Admin" | "Personel"
    public bool IsActive { get; set; } = true;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
}
