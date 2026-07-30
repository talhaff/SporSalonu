using SporSalonu.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace SporSalonu.Infrastructure.Services;

public class MockTurnstileService : ITurnstileService
{
    private readonly ILogger<MockTurnstileService> _logger;

    public MockTurnstileService(ILogger<MockTurnstileService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> OpenGateAsync(bool isEntry = true)
    {
        var direction = isEntry ? "Giriş" : "Çıkış";
        _logger.LogInformation($"MOCK: Turnike rölesi tetiklendi, {direction} kapısı açıldı.");
        
        // Simüle edilmiş donanım gecikmesi
        await Task.Delay(200);
        
        return true;
    }
}
