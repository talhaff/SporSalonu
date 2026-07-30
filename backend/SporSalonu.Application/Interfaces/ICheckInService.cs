using SporSalonu.Application.DTOs.CheckIn;

namespace SporSalonu.Application.Interfaces;

public interface ICheckInService
{
    /// <summary>
    /// Üyenin giriş yapıp yapamayacağını kontrol eder.
    /// Kontroller: 1) Aktif üyelik var mı? 2) Dondurulmuş mu? 3) Borcu var mı?
    /// Sonucu CheckInLog tablosuna kaydeder.
    /// </summary>
    Task<CheckInResultDto> ProcessCheckInAsync(CheckInRequestDto dto);

    Task<List<CheckInResultDto>> GetTodayCheckInsAsync();

    Task<List<CheckInHistoryDto>> GetCheckInHistoryAsync();
}
