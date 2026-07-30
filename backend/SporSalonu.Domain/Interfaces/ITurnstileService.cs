namespace SporSalonu.Domain.Interfaces;

public interface ITurnstileService
{
    /// <summary>
    /// Turnike kapısını açar. isEntry true ise giriş, false ise çıkış kapısı açılır.
    /// </summary>
    /// <param name="isEntry">Giriş için mi (true), Çıkış için mi (false)?</param>
    /// <returns>İşlem başarılıysa true döner.</returns>
    Task<bool> OpenGateAsync(bool isEntry = true);
}
