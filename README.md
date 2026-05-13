# 📚 Dictionary API

.NET 9 ile geliştirilmiş, **Clean Architecture (Onion Architecture prensipleri)** kullanan ölçeklenebilir bir RESTful API projesidir.

Proje, modüler yapı, sürdürülebilirlik ve kurumsal backend mimarisi hedeflenerek geliştirilmiştir.

---

## 🚀 Kullanılan Teknolojiler

### Backend
- .NET 9 Web API
- Entity Framework Core
- ASP.NET Core Identity
- JWT Authentication
- Clean Architecture (Onion Architecture prensipleri)
- Options Pattern
- Dependency Injection (DI)
- SQL Server (LocalDB / MSSQL)
- Soft Delete Interceptor

---

## 🧱 Mimari Yapı

Proje Clean Architecture prensiplerine göre katmanlı olarak tasarlanmıştır:





### Katmanlar

- **Domain** → Temel entity’ler ve iş kuralları
- **Application** → Use-case’ler, DTO’lar, interface’ler
- **Infrastructure** → JWT, dış servisler
- **Persistence** → EF Core, DbContext, konfigürasyonlar
- **API** → Controller katmanı

---

## 🔐 Kimlik Doğrulama (JWT)

JWT tabanlı authentication sistemi kullanılmıştır:

- Access Token: 15 dakika
- Refresh Token: 7 gün
- Role bazlı yetkilendirme (Admin, Editor)

### Token içerisinde bulunan bilgiler:
- UserId
- Email
- Username
- FullName
- Roller

---

## 👤 Seed Kullanıcı

Proje ilk çalıştığında otomatik olarak bir admin kullanıcı oluşturulur:


## 🧠 Domain Model Yapısı

Bu proje, dilbilimsel veriyi çok katmanlı ve ilişkisel bir model üzerinden temsil eder. Amaç yalnızca kelime saklamak değil; kelimenin anlam, bağlam, telaffuz, etimoloji ve ilişkilerini yönetebilecek bir sözlük altyapısı oluşturmaktır.

---

## 🧱 Temel Tasarım Yaklaşımı

Proje domain katmanı şu prensiplere göre tasarlanmıştır:

- **Normalized lexical database structure**
- **Soft delete destekli veri yönetimi**
- **Çok dilli (multi-language) destek**
- **Graph-based meaning relationships**
- **Audit/history tracking**
- **Extensible linguistic model**

---

## 🧩 Base Yapı

### BaseEntity
Tüm entity’ler için temel kimlik yapısı:

- Id

### SoftDeletableEntity
Silme işlemleri fiziksel değil mantıksal yapılır:

- IsDeleted
- DeletedAt

Bu sayede:
- veri kaybı olmaz
- geri alma (restore) mümkündür

---

## 🌍 Dil Modeli

### Language
Sistemdeki dilleri temsil eder.

Örnek:
- tr → Türkçe
- ko → Korece

### Script
Dil yazı sistemlerini temsil eder:

- Latin
- Hangul
- Hanja

---

## 📚 Lexical Core

### Entry (Kelime)
Sistemin ana birimidir.

Bir kelime:
- birden fazla anlam (Sense)
- birden fazla telaffuz
- çekimli formlar içerebilir

Özellikler:
- Headword
- Normalized form
- LanguageId

---

### Pronunciation
Kelimenin farklı okunuşlarını temsil eder:

- IPA
- Romanization
- Native pronunciation

---

### EntryForm
Morfolojik yapıyı temsil eder:

- çekimli formlar
- türemiş kelimeler
- dil bilgisel varyasyonlar

---

## 🧠 Anlam Katmanı

### Sense
Kelimenin her bir anlamı ayrı bir atomik birimdir.

Özellikler:
- Part of Speech (noun, verb, etc.)
- Definitions (çok dilli açıklama)
- Order (öncelik sırası)
- Child senses (hiyerarşik yapı)

---

### SenseDefinition
Bir anlamın farklı dillerde açıklamaları.

---

### SenseTranslation
Sense’ler arası yönlü çeviri ilişkisi:

- A → B tek yönlü tutulur
- çift yönlü query application layer’da çözülür
- confidence score ile semantik doğruluk ölçülür

---

### SenseRelation
Anlamlar arası semantic graph ilişkisi:

- Synonym (eş anlamlı)
- Antonym (zıt anlamlı)
- Related (ilişkili)
- Hypernym / Hyponym (üst-alt kavram)

---

## 🏷️ Etiketleme Sistemi

### Label & LabelType
Anlamlara bağlam kazandırır:

- register (resmi / argo)
- domain (tıp, hukuk, yemek)
- region (lehçe)

Many-to-many yapı:

- Sense ↔ Label (SenseLabel)


## 📖 Örnek Cümle Sistemi

### Example
- source text
- translations (çok dilli destek)

---

## 🧬 Etimoloji

### Etymology
Kelimenin köken bilgisini tutar:

- Source language
- Orijinal form
- anlam açıklaması
- tarihsel zincir


## 🔥 Mimari Güçlü Noktalar

- Çok katmanlı anlam modeli
- Graph-based lexical relationships
- Multi-language support
- Full audit trail
- Soft delete safety
- Extensible design (yeni dil / yapı eklemek kolay)

---

## 🎯 Amaç

Bu domain modeli:

> “Sadece sözlük değil, dilin kendisini modelleyen bir sistem oluşturmayı hedefler.”
