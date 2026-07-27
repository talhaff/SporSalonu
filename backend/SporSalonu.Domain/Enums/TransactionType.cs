namespace SporSalonu.Domain.Enums;

public enum TransactionType
{
    Borclandirma = 1,   // Paket alındığında borç
    Odeme = 2,           // Nakit/kart ödeme
    Iade = 3,            // İade
    Indirim = 4          // Manuel indirim kaydı
}
