# ReservationSystem - Etkinlik ve Rezervasyon Yönetim Sistemi API'si

Bu proje, ASP.NET Core kullanılarak geliştirilmiş, etkinlik ve rezervasyon yönetimini sağlayan bir API'dir. Temel Onion Mimarisi prensiplerine uygun olarak tasarlanmıştır ve PostgreSQL veritabanı ile Entity Framework Core ORM'i kullanır. Kullanıcıların kayıt olmasını, giriş yapmasını, mevcut etkinlikleri görüntülemesini, etkinlikler için rezervasyon yapmasını ve bu rezervasyonları yönetmesini sağlar.

## 🚀 Genel Bakış

Proje, kullanıcıların etkinliklere kolayca yer ayırmasını ve etkinlik sahiplerinin etkinliklerini ve kapasitelerini yönetmesini sağlayan temel bir API arayüzü sunar. Rezervasyon süreci "Hold" ve "Confirm" olmak üzere iki aşamalıdır ve otomatik iptal mekanizması içerir.

## 🏗️ Mimari

Proje, yazılımın katmanlar arasında temiz bir ayrımını sağlayan ve bağımlılıkları dışa doğru yönlendiren **Onion Mimarisi** prensiplerini takip eder.

*   **ReservationSystem.Domain:** Uygulamanın kalbi olan iş varlıklarını (`Event`, `Reservation`, `ApplicationUser`) ve iş kurallarını (örn. `ReservationStatus` enum) tanımlar.
*   **ReservationSystem.Application:** Uygulamaya özgü iş mantığını, DTO'ları (Data Transfer Objects), ve depolar (repository) için arayüzleri içerir. API ile Domain/Persistence katmanları arasındaki etkileşimleri yönetir.
*   **ReservationSystem.Persistence:** Entity Framework Core ve PostgreSQL kullanarak veri erişim mantığını uygular. Application katmanında tanımlanan repository arayüzlerini somutlaştırır ve `ApplicationDbContext`'i barındırır.
*   **ReservationSystem.Infrastructure:** (Şu an için `ReservationCleanupService` arka plan servisini içerir) Genellikle e-posta, SMS bildirimleri veya harici ödeme entegrasyonları gibi dış servislerin uygulamalarını içerir.
*   **ReservationSystem.API:** Uygulamanın giriş noktasıdır. İstemci uygulamalar için HTTP uç noktalarını ifşa eder, istekleri işler, girdiyi doğrular ve Application katmanındaki ilgili servisleri çağırır. ASP.NET Core Identity ve Swagger UI yapılandırmasını içerir.

## 💻 Teknolojiler

*   **ASP.NET Core 8.0:** Web API çerçevesi.
*   **Entity Framework Core 8.0:** Veritabanı etkileşimi için ORM.
*   **PostgreSQL:** İlişkisel veritabanı sistemi.
*   **Microsoft.AspNetCore.Identity.EntityFrameworkCore:** Kullanıcı yönetimi (kayıt, giriş) için.
*   **Swashbuckle.AspNetCore:** API dokümantasyonu ve test için Swagger/OpenAPI UI.
*   **Newtonsoft.Json:** JSON serileştirme ve seri durumdan çıkarma.

## ✨ Özellikler

*   **Kullanıcı Yönetimi:** Temel kullanıcı kaydı ve giriş işlevselliği.
*   **Etkinlik Yönetimi:** Etkinliklerin oluşturulması, görüntülenmesi, güncellenmesi ve silinmesi (CRUD) işlemleri.
*   **Rezervasyon Yönetimi:**
    *   **Hold (Geçici Ayırma):** Kullanıcı bir rezervasyon talebi gönderdiğinde, 5 dakikalık bir geçici ayırma oluşturulur. Bu süre boyunca kapasite geçici olarak düşer.
    *   **Confirm (Onaylama):** Kullanıcı 5 dakika içinde rezervasyonu onaylayabilir. Onaylandığında rezervasyon durumu `CONFIRMED` olur ve kapasite kalıcı olarak düşer.
    *   **Otomatik İptal:** 5 dakikalık hold süresi dolan bekleyen rezervasyonlar, arka plan servisi aracılığıyla otomatik olarak iptal edilir ve kapasite etkinliğe geri iade edilir.
*   **Swagger UI:** API uç noktalarını keşfetmek ve test etmek için etkileşimli dokümantasyon arayüzü.

## ⚙️ Kurulum

### Projenin Klonlanması

```bash
git clone <proje-deposu-url>
cd ReservationSystem
```

### Kimlik Doğrulama (`/api/Auth`)

*   **`POST /api/Auth/register`**: Yeni bir kullanıcı hesabı oluşturur.
*   **`POST /api/Auth/login`**: Mevcut bir kullanıcıyı doğrular.

### Etkinlikler (`/api/Events`)

*   **`GET /api/Events`**: Mevcut tüm etkinliklerin listesini döndürür.
*   **`GET /api/Events/{id}`**: Belirli bir etkinlik ID'sine göre etkinliğin detaylarını döndürür.
*   **`POST /api/Events`**: Yeni bir etkinlik oluşturur.
*   **`PUT /api/Events/{id}`**: Belirli bir etkinlik ID'sine göre etkinliği günceller.
*   **`DELETE /api/Events/{id}`**: Belirli bir etkinlik ID'sine göre etkinliği siler.

### Rezervasyonlar (`/api/Reservations`)

*   **`GET /api/Reservations`**: Sistemdeki tüm rezervasyonların listesini döndürür.
*   **`GET /api/Reservations/{id}`**: Belirli bir rezervasyon ID'sine göre rezervasyon detaylarını döndürür.
*   **`POST /api/Reservations`**: Bir etkinlik için yeni bir rezervasyon talebi oluşturur (HOLD durumu).
*   **`POST /api/Reservations/{id}/cancel`**: Belirli bir rezervasyon ID'sine göre rezervasyonu iptal eder.
*   **`POST /api/Reservations/{id}/confirm`**: Beklemede (Pending) durumundaki bir rezervasyonu onaylar. Hold süresi dolmuş rezervasyonlar onaylanamaz.

## 🕒 Rezervasyon Akışı Detayları (Hold, Confirm, Auto-Cancel)

Rezervasyon süreci, etkinliğin kapasitesini yönetmek için iki ana aşama içerir:

1.  **HOLD (Geçici Ayırma):**
    *   Kullanıcı `POST /api/Reservations` uç noktasına bir rezervasyon talebi gönderdiğinde, rezervasyon `Pending` (Beklemede) durumunda oluşturulur.
    *   `HoldUntil` adında bir zaman damgası, rezervasyonun oluşturulduğu andan itibaren 5 dakika sonrasına ayarlanır.
    *   İlgili etkinliğin `AvailableTickets` kapasitesi, rezervasyon yapılan bilet sayısı kadar **geçici olarak düşer**.
    *   Bu aşamada rezervasyon hala onaylanmamıştır ve 5 dakika içinde onaylanmazsa iptal edilecektir.

2.  **CONFIRM (Onaylama):**
    *   Kullanıcı `POST /api/Reservations/{id}/confirm` uç noktasına 5 dakikalık hold süresi içinde istek gönderirse, rezervasyon `CONFIRMED` (Onaylandı) durumuna güncellenir.
    *   Kapasite düşüşü bu noktada kalıcı hale gelir.

3.  **OTOMATİK İPTAL (Hold Süresi Dolarsa):**
    *   Uygulama arka planında çalışan `ReservationCleanupService` adında bir servis bulunmaktadır.
    *   Bu servis her dakika çalışır ve veritabanındaki tüm `Pending` durumundaki rezervasyonları kontrol eder.
    *   Eğer bir `Pending` rezervasyonun `HoldUntil` zamanı geçmişse (yani 5 dakikalık bekleme süresi dolmuşsa):
        *   Bu rezervasyon otomatik olarak `Cancelled` (İptal Edildi) durumuna güncellenir.
        *   İptal edilen rezervasyonun bilet adedi, ilgili etkinliğin `AvailableTickets` kapasitesine **geri iade edilir**.
    *   Bu sayede, süresi dolan geçici rezervasyonlar sistemden otomatik olarak temizlenir ve biletler tekrar kullanılabilir hale gelir.

