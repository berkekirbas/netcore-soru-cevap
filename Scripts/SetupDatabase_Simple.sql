-- =============================================
-- BASİT VERSİYON - Id kolonu OLMADAN
-- UserRoles tablosunda Id kolonu yoksa bu scripti kullanın
-- =============================================

USE SoruCevap;
GO

-- 1. Admin Rolü Ekle
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
BEGIN
    INSERT INTO Roles (Name, CreatedAt, UpdatedAt) 
    VALUES ('Admin', GETDATE(), NULL);
    PRINT 'Admin rolü eklendi.';
END
ELSE
BEGIN
    PRINT 'Admin rolü zaten mevcut.';
END
GO

-- 2. Admin Kullanıcısı Ekle
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Email, Password, FullName, IsActive, CreatedAt, UpdatedAt)
    VALUES ('admin', 'admin@example.com', 'admin123', 'Admin User', 1, GETDATE(), NULL);
    PRINT 'Admin kullanıcısı eklendi.';
END
ELSE
BEGIN
    PRINT 'Admin kullanıcısı zaten mevcut.';
END
GO

-- 3. Admin Kullanıcısına Admin Rolünü Ata (Id kolonu OLMADAN)
-- Önce değişkenlere değerleri al
DECLARE @UserId INT;
DECLARE @RoleId INT;

-- Admin kullanıcısının Id'sini al (ilk kaydı al)
SELECT TOP 1 @UserId = Id FROM Users WHERE Username = 'admin';

-- Admin rolünün Id'sini al (ilk kaydı al)
SELECT TOP 1 @RoleId = Id FROM Roles WHERE Name = 'Admin';

-- Kontrol et ve ekle
IF @UserId IS NOT NULL AND @RoleId IS NOT NULL
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM UserRoles 
        WHERE UserId = @UserId
        AND RoleId = @RoleId
    )
    BEGIN
        INSERT INTO UserRoles (UserId, RoleId, CreatedAt, UpdatedAt)
        VALUES (@UserId, @RoleId, GETDATE(), NULL);
        PRINT 'Admin kullanıcısına Admin rolü atandı.';
    END
    ELSE
    BEGIN
        PRINT 'Admin kullanıcısına Admin rolü zaten atanmış.';
    END
END
ELSE
BEGIN
    PRINT 'HATA: Admin kullanıcısı veya Admin rolü bulunamadı!';
    PRINT 'UserId: ' + ISNULL(CAST(@UserId AS VARCHAR), 'NULL');
    PRINT 'RoleId: ' + ISNULL(CAST(@RoleId AS VARCHAR), 'NULL');
END
GO

PRINT '';
PRINT '========================================';
PRINT 'Kurulum tamamlandı!';
PRINT 'Giriş Bilgileri:';
PRINT 'Kullanıcı Adı: admin';
PRINT 'Şifre: admin123';
PRINT '========================================';
GO

