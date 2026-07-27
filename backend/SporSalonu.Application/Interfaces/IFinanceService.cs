using SporSalonu.Application.DTOs.Finance;

namespace SporSalonu.Application.Interfaces;

public interface IFinanceService
{
    /// <summary>Üyenin tüm finansal hareketleri ve bakiyesi.</summary>
    Task<MemberFinanceSummaryDto> GetMemberFinanceSummaryAsync(int memberId);

    /// <summary>Ödeme kaydı oluşturur, bakiyeyi günceller.</summary>
    Task RecordPaymentAsync(CreatePaymentDto dto);

    /// <summary>Tüm sistemdeki finansal hareketleri ve kasa özetini getirir.</summary>
    Task<GlobalFinanceSummaryDto> GetGlobalFinanceSummaryAsync();
}
