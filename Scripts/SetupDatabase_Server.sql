-- =============================================
-- Soru Cevap Portal - Veritabanı Kurulum Scripti
-- Uzak Sunucuda Çalıştırın
-- =============================================

-- Veritabanını kullan (SoruCevap veya SoruCevapPortal - hangisi varsa)
USE SoruCevap;
GO

-- Eğer SoruCevap yoksa SoruCevapPortal'ı dene
-- USE SoruCevapPortal;
-- GO

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
-- Önce UserRoles tablosunun yapısını kontrol et
DECLARE @HasIdColumn BIT = 0;
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserRoles' AND COLUMN_NAME = 'Id')
BEGIN
    SET @HasIdColumn = 1;
END
GO

-- İlişkiyi kontrol et ve ekle
IF NOT EXISTS (
    SELECT 1 FROM UserRoles 
    WHERE UserId = (SELECT Id FROM Users WHERE Username = 'admin')
    AND RoleId = (SELECT Id FROM Roles WHERE Name = 'Admin')
)
BEGIN
    -- Id kolonu kontrolü için dinamik SQL kullan
    DECLARE @UserId INT = (SELECT Id FROM Users WHERE Username = 'admin');
    DECLARE @RoleId INT = (SELECT Id FROM Roles WHERE Name = 'Admin');
    DECLARE @HasId BIT = 0;
    
    -- Id kolonu var mı kontrol et
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserRoles' AND COLUMN_NAME = 'Id')
    BEGIN
        SET @HasId = 1;
    END
    
    IF @HasId = 1
    BEGIN
        -- Id kolonu VARSA
        DECLARE @NewId INT = ISNULL((SELECT MAX(Id) FROM UserRoles), 0) + 1;
        INSERT INTO UserRoles (UserId, RoleId, Id, CreatedAt, UpdatedAt)
        VALUES (@UserId, @RoleId, @NewId, GETDATE(), NULL);
        PRINT 'Admin kullanıcısına Admin rolü atandı (Id ile).';
    END
    ELSE
    BEGIN
        -- Id kolonu YOKSA
        INSERT INTO UserRoles (UserId, RoleId, CreatedAt, UpdatedAt)
        VALUES (@UserId, @RoleId, GETDATE(), NULL);
        PRINT 'Admin kullanıcısına Admin rolü atandı (Id olmadan).';
    END
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

