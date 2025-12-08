-- =============================================
-- MİNİMAL VERİ - Sadece Admin Girişi İçin
-- Migration'dan SONRA çalıştırın
-- =============================================

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

-- 3. Admin Kullanıcısına Admin Rolünü Ata
-- NOT: UserRoles tablosunda Id kolonu var ama Identity değil
IF NOT EXISTS (
    SELECT 1 FROM UserRoles 
    WHERE UserId = (SELECT Id FROM Users WHERE Username = 'admin')
    AND RoleId = (SELECT Id FROM Roles WHERE Name = 'Admin')
)
BEGIN
    DECLARE @UserId INT = (SELECT Id FROM Users WHERE Username = 'admin');
    DECLARE @RoleId INT = (SELECT Id FROM Roles WHERE Name = 'Admin');
    DECLARE @MaxId INT = ISNULL((SELECT MAX(Id) FROM UserRoles), 0);
    
    INSERT INTO UserRoles (UserId, RoleId, Id, CreatedAt, UpdatedAt)
    VALUES (@UserId, @RoleId, @MaxId + 1, GETDATE(), NULL);
    PRINT 'Admin kullanıcısına Admin rolü atandı.';
END
ELSE
BEGIN
    PRINT 'Admin kullanıcısına Admin rolü zaten atanmış.';
END

-- =============================================
-- Giriş Bilgileri:
-- Kullanıcı Adı: admin
-- Şifre: admin123
-- =============================================

