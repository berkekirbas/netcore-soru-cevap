-- =============================================
-- Soru Cevap Portal - İlk Veri Ekleme Scripti
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

-- 2. User Rolü Ekle (opsiyonel)
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'User')
BEGIN
    INSERT INTO Roles (Name, CreatedAt, UpdatedAt) 
    VALUES ('User', GETDATE(), NULL);
    PRINT 'User rolü eklendi.';
END
ELSE
BEGIN
    PRINT 'User rolü zaten mevcut.';
END

-- 3. Admin Kullanıcısı Ekle
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Email, Password, FullName, IsActive, CreatedAt, UpdatedAt)
    VALUES ('admin', 'admin@example.com', 'admin123', 'Admin User', 1, GETDATE(), NULL);
    PRINT 'Admin kullanıcısı eklendi.';
    
    -- Admin kullanıcısına Admin rolünü ata
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
    PRINT 'Admin kullanıcısı zaten mevcut.';
END

-- 4. Örnek Kategoriler Ekle (opsiyonel)
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Genel')
BEGIN
    INSERT INTO Categories (Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES ('Genel', 'Genel sorular için kategori', 1, GETDATE(), NULL);
    PRINT 'Genel kategorisi eklendi.';
END

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Teknoloji')
BEGIN
    INSERT INTO Categories (Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES ('Teknoloji', 'Teknoloji ile ilgili sorular', 1, GETDATE(), NULL);
    PRINT 'Teknoloji kategorisi eklendi.';
END

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Programlama')
BEGIN
    INSERT INTO Categories (Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES ('Programlama', 'Programlama dilleri ve teknikleri', 1, GETDATE(), NULL);
    PRINT 'Programlama kategorisi eklendi.';
END

-- =============================================
-- ÖNEMLİ: Admin Giriş Bilgileri
-- =============================================
PRINT '';
PRINT '========================================';
PRINT 'Admin Giriş Bilgileri:';
PRINT 'Kullanıcı Adı: admin';
PRINT 'Şifre: admin123';
PRINT '========================================';
PRINT '';
