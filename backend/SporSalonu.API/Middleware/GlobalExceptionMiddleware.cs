using System.Net;
using System.Text.Json;

namespace SporSalonu.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            await YazHata(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await YazHata(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await YazHata(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await YazHata(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen hata: {Message}", ex.Message);
            await YazHata(context, HttpStatusCode.InternalServerError, "Sunucu hatası oluştu. Lütfen tekrar deneyin.");
        }
    }

    private static async Task YazHata(HttpContext context, HttpStatusCode statusCode, string mesaj)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var hata = new
        {
            basarili = false,
            hataKodu = (int)statusCode,
            mesaj
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(hata, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
