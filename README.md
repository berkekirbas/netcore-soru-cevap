# Soru Cevap Portalı - Admin Panel

ASP.NET Core MVC (.NET 8) ile geliştirilmiş soru-cevap portalı admin paneli.

## Özellikler

- ✅ Cookie tabanlı kimlik doğrulama
- ✅ Repository Pattern implementasyonu
- ✅ Entity Framework Core ile MSSQL entegrasyonu
- ✅ Admin paneli (Bootstrap 4+)
- ✅ CRUD işlemleri (Category, Question, Answer, User)
- ✅ AJAX ile dinamik işlemler
- ✅ Rol tabanlı yetkilendirme

## Kurulum

### 1. Connection String Ayarlama

`appsettings.json` dosyasındaki connection string'i kendi MSSQL sunucu bilgilerinizle güncelleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=SoruCevapPortal;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}
```

### 2. Migration İşlemleri

Proje klasöründe aşağıdaki komutları çalıştırın:

```bash
# Initial migration oluştur
dotnet ef migrations add InitialCreate

# Database'i güncelle
dotnet ef database update
```

### 3. İlk Admin Kullanıcısı Oluşturma

Migration'dan sonra, veritabanında manuel olarak veya SQL script ile:

1. **Role** tablosuna "Admin" rolü ekleyin:

```sql
INSERT INTO Roles (Name, CreatedAt, UpdatedAt)
VALUES ('Admin', GETDATE(), NULL);
```

2. **User** tablosuna admin kullanıcısı ekleyin:

```sql
INSERT INTO Users (Username, Email, Password, FullName, IsActive, CreatedAt, UpdatedAt)
VALUES ('admin', 'admin@example.com', 'admin123', 'Admin User', 1, GETDATE(), NULL);
```

3. **UserRole** tablosuna kullanıcı-rol ilişkisini ekleyin:

```sql
INSERT INTO UserRoles (UserId, RoleId, CreatedAt, UpdatedAt)
VALUES (
    (SELECT Id FROM Users WHERE Username = 'admin'),
    (SELECT Id FROM Roles WHERE Name = 'Admin'),
    GETDATE(), NULL
);
```

## Kullanım

1. Projeyi çalıştırın:

```bash
dotnet run
```

2. Tarayıcıda `/Account/Login` adresine gidin

3. Admin kullanıcı adı ve şifresi ile giriş yapın:

   - Kullanıcı Adı: `admin`
   - Şifre: `admin123`

4. Admin panelinde:
   - **Dashboard**: Genel istatistikler
   - **Kategoriler**: Kategori CRUD işlemleri (AJAX delete)
   - **Sorular**: Soru listesi ve aktif/pasif toggle (AJAX)
   - **Cevaplar**: Cevap listesi ve onay toggle (AJAX)
   - **Kullanıcılar**: Kullanıcı listesi

## Proje Yapısı

```
SoruCevapPortal/
├── Controllers/
│   ├── AccountController.cs       # Login/Logout
│   ├── AdminController.cs         # Dashboard
│   ├── AdminCategoryController.cs # Category CRUD
│   ├── AdminQuestionController.cs # Question yönetimi
│   ├── AdminAnswerController.cs   # Answer yönetimi
│   └── AdminUserController.cs     # User listesi
├── Models/
│   ├── BaseEntity.cs
│   ├── User.cs
│   ├── Role.cs
│   ├── UserRole.cs
│   ├── Category.cs
│   ├── Question.cs
│   ├── Answer.cs
│   └── QuestionVote.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Repositories/
│   ├── IGenericRepository.cs
│   ├── GenericRepository.cs
│   ├── IUnitOfWork.cs
│   ├── UnitOfWork.cs
│   └── [Specific Repositories]
└── Views/
    ├── Shared/
    │   └── _AdminLayout.cshtml
    └── [Controller Views]
```

## Güvenlik

- Tüm admin controller'lar `[Authorize(Policy = "AdminOnly")]` ile korunmaktadır
- Cookie authentication 24 saat süreyle geçerlidir
- Sadece "Admin" rolüne sahip kullanıcılar admin paneline erişebilir

## Notlar

- Şifreler şu anda plain text olarak saklanmaktadır. Production'da mutlaka hash'lenmelidir.
