<div align="center">
  <br />
  <h1>🏋️ VipGym - Modern Spor Salonu Yönetim Sistemi</h1>
  <p>
    <strong>Yüksek performanslı, ölçeklenebilir ve premium tasarımlı yeni nesil spor salonu yönetim platformu.</strong>
  </p>
  <br />
</div>

<details open>
  <summary>Tablo</summary>
  <ol>
    <li><a href="#proje-hakkında">Proje Hakkında</a></li>
    <li><a href="#özellikler">Özellikler</a></li>
    <li><a href="#teknoloji-yığını-tech-stack">Teknoloji Yığını</a></li>
    <li><a href="#mimari">Mimari</a></li>
    <li><a href="#kurulum-ve-çalıştırma">Kurulum ve Çalıştırma</a></li>
  </ol>
</details>

## 🚀 Proje Hakkında
VipGym, spor salonlarının üye takiplerini, finansal kayıtlarını, check-in işlemlerini ve abonelik paketlerini uçtan uca modern bir şekilde yönetmesini sağlayan kapsamlı bir sistemdir. Kullanıcı deneyimini odağına alan "Glassmorphism" UI yaklaşımı ve Clean Architecture prensipleriyle tasarlanmış, sağlam bir altyapıya sahiptir.

## ✨ Özellikler

- 📊 **Gelişmiş Dashboard:** Anlık aktif üyeler, aylık gelir tablosu, günlük check-in hareketleri ve yaklaşan üyelik bitişlerinin tek ekranda takibi.
- 👥 **Müşteri & Abonelik Yönetimi:** Üyelerin kayıt süreçleri, abonelik paketi atamaları, indirim tanımlamaları ve üyelik dondurma/uzatma işlemleri.
- 💰 **Kasa ve Finans:** Kredi kartı/Nakit/Havale takip sistemi, borçlandırma mekanizması, ödeme geçmişi ve detaylı finansal döküm.
- 🛂 **QR / Manuel Check-in:** Üyelerin salona giriş izinlerinin anlık olarak bakiyesine ve üyelik süresine göre değerlendirilip onay/red verilmesi.
- 🔔 **Akıllı Bildirim Sistemi (Hangfire):** Arka planda periyodik olarak çalışan görevler sayesinde aboneliği bitmek üzere olan üyelerin tespiti ve sisteme bildirim düşülmesi.
- 🔐 **Güvenlik (JWT):** Token tabanlı yetkilendirme ile güvenli yönetici oturumları.

## 🛠 Teknoloji Yığını (Tech Stack)

### Arka Uç (Backend)
- **Framework:** .NET 8 (ASP.NET Core Web API)
- **Mimari:** Clean Architecture
- **ORM:** Entity Framework Core
- **Veritabanı:** PostgreSQL
- **Arka Plan Görevleri:** Hangfire
- **Kimlik Doğrulama:** JWT (JSON Web Token)
- **Loglama & Güvenlik:** Global Exception Handling, CORS

### Ön Yüz (Frontend)
- **Framework:** Next.js 14 (App Router)
- **Dil:** TypeScript
- **Stilizasyon:** Tailwind CSS
- **Tasarım Dili:** Glassmorphism (Modern, yarı saydam premium arayüz)
- **Veri Çekme:** React Query (TanStack Query) & Axios
- **İkonlar:** Lucide React

## 🏗 Mimari
Backend projesi "Clean Architecture" prensiplerine uygun olarak birbirinden bağımsız 4 katman şeklinde tasarlanmıştır:
1. **Domain:** Çekirdek iş kuralları, varlıklar (Entities) ve Enums.
2. **Application:** DTO'lar ve Interface'ler (Soyutlamalar).
3. **Infrastructure:** Veritabanı işlemleri (AppDbContext), servis implementasyonları ve Hangfire entegrasyonu.
4. **API:** Controller'lar, Middleware'ler ve Dependency Injection (Bağımlılık Enjeksiyonu).

## 💻 Kurulum ve Çalıştırma

### Gereksinimler
- Node.js (v18+)
- .NET 8 SDK
- PostgreSQL

### 1. Veritabanı Yapılandırması
`backend/SporSalonu.API/appsettings.json` dosyasındaki veritabanı bağlantı cümlenizi (`DefaultConnection`) kendi PostgreSQL sunucunuza göre düzenleyin.

### 2. Backend'i Başlatma
```bash
cd backend
dotnet build SporSalonu.slnx
cd SporSalonu.API
dotnet run
```
Backend `http://localhost:5000` adresinde çalışacaktır. (Swagger için: `http://localhost:5000/swagger`)

### 3. Frontend'i Başlatma
```bash
cd frontend/spor-salonu-ui
npm install
npm run dev
```
Uygulamaya `http://localhost:3000` adresinden erişebilirsiniz.

---
*Bu proje modern web standartlarında "Premium" bir deneyim sunmak hedefiyle özenle kodlanmıştır.*
