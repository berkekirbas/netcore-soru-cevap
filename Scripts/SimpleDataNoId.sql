-- =============================================
-- BASİT VERİ EKLEME - Id KOLONU OLMADAN
-- UserRoles tablosunda Id kolonu yoksa bu scripti kullanın
-- =============================================

-- Adım 1: Admin Rolü
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

-- Adım 2: Admin Kullanıcısı
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

-- Adım 3: Admin Kullanıcısına Admin Rolünü Ata
-- UserRoles tablosunda Id kolonu YOKSA bu versiyonu kullanın
IF NOT EXISTS (
    SELECT 1 FROM UserRoles 
    WHERE UserId = (SELECT Id FROM Users WHERE Username = 'admin')
    AND RoleId = (SELECT Id FROM Roles WHERE Name = 'Admin')
)
BEGIN
    INSERT INTO UserRoles (UserId, RoleId, CreatedAt, UpdatedAt)
    VALUES (
        (SELECT Id FROM Users WHERE Username = 'admin'),
        (SELECT Id FROM Roles WHERE Name = 'Admin'),
        GETDATE(), NULL
    );
    PRINT 'Admin kullanıcısına Admin rolü atandı.';
END
ELSE
BEGIN
    PRINT 'Admin kullanıcısına Admin rolü zaten atanmış.';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'İşlem tamamlandı!';
PRINT 'Kullanıcı Adı: admin';
PRINT 'Şifre: admin123';
PRINT '========================================';
GO

