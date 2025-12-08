-- =============================================
-- Soru Cevap Portal - Veritabanı Kurulum Scripti
-- sqlcmd ile çalıştırın
-- =============================================

USE SoruCevapPortal;
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

-- 3. Admin Kullanıcısına Admin Rolünü Ata
-- UserRoles tablosunun yapısına göre (Id kolonu varsa veya yoksa)
IF NOT EXISTS (
    SELECT 1 FROM UserRoles 
    WHERE UserId = (SELECT Id FROM Users WHERE Username = 'admin')
    AND RoleId = (SELECT Id FROM Roles WHERE Name = 'Admin')
)
BEGIN
    DECLARE @UserId INT = (SELECT Id FROM Users WHERE Username = 'admin');
    DECLARE @RoleId INT = (SELECT Id FROM Roles WHERE Name = 'Admin');
    
    -- Önce UserRoles tablosunun yapısını kontrol et
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserRoles' AND COLUMN_NAME = 'Id')
    BEGIN
        -- Id kolonu varsa
        DECLARE @NewId INT = ISNULL((SELECT MAX(Id) FROM UserRoles), 0) + 1;
        INSERT INTO UserRoles (UserId, RoleId, Id, CreatedAt, UpdatedAt)
        VALUES (@UserId, @RoleId, @NewId, GETDATE(), NULL);
    END
    ELSE
    BEGIN
        -- Id kolonu yoksa
        INSERT INTO UserRoles (UserId, RoleId, CreatedAt, UpdatedAt)
        VALUES (@UserId, @RoleId, GETDATE(), NULL);
    END
    
    PRINT 'Admin kullanıcısına Admin rolü atandı.';
END
ELSE
BEGIN
    PRINT 'Admin kullanıcısına Admin rolü zaten atanmış.';
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

