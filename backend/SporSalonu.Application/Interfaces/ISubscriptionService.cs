using SporSalonu.Application.DTOs.Subscription;
using SporSalonu.Domain.Entities;

namespace SporSalonu.Application.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionDetailDto?> GetByIdAsync(int id);
    Task<List<SubscriptionDetailDto>> GetByMemberIdAsync(int memberId);
    
    /// <summary>Bitişine 7 gün veya daha az kalmış aktif üyelikleri getirir.</summary>
    Task<List<SubscriptionDetailDto>> GetExpiringSubscriptionsAsync();

    /// <summary>
    /// Müşteriye paket atar. Borçlandırma ve peşinat TransactionLog'a yazılır.
    /// </summary>
    Task<int> CreateSubscriptionAsync(CreateSubscriptionDto dto);

    /// <summary>
    /// Üyeliği dondurur: BitisTarihi += GunSayisi ve FreezeLog oluşturur.
    /// </summary>
    Task FreezeAsync(FreezeSubscriptionDto dto);

    /// <summary>
    /// Süresi dolan abonelikleri Pasif yapan job metodu.
    /// </summary>
    Task ProcessExpiredSubscriptionsAsync();

    /// <summary>
    /// Bitişine 3 gün kalan üyeler için bildirim oluşturan job metodu.
    /// </summary>
    Task SendExpiryNotificationsAsync();
}
