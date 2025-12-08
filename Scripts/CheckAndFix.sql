-- =============================================
-- KONTROL VE DÜZELTME SCRIPTİ
-- Önce mevcut durumu kontrol eder, sonra düzeltir
-- =============================================

USE SoruCevap;
GO

-- 1. Mevcut durumu kontrol et
PRINT '=== MEVCUT DURUM KONTROLÜ ===';
PRINT '';

-- Admin kullanıcıları
PRINT 'Admin kullanıcıları:';
SELECT Id, Username, Email FROM Users WHERE Username = 'admin';
PRINT '';

-- Admin rolleri
PRINT 'Admin rolleri:';
SELECT Id, Name FROM Roles WHERE Name = 'Admin';
PRINT '';

-- Mevcut UserRoles ilişkileri
PRINT 'Mevcut UserRoles ilişkileri:';
SELECT ur.UserId, ur.RoleId, u.Username, r.Name AS RoleName
FROM UserRoles ur
INNER JOIN Users u ON ur.UserId = u.Id
INNER JOIN Roles r ON ur.RoleId = r.Id
WHERE u.Username = 'admin' AND r.Name = 'Admin';
PRINT '';

-- 2. Eğer birden fazla admin kullanıcısı varsa, ilkini kullan
DECLARE @UserId INT;
DECLARE @RoleId INT;

-- İlk admin kullanıcısını al
SELECT TOP 1 @UserId = Id FROM Users WHERE Username = 'admin' ORDER BY Id;

-- İlk admin rolünü al
SELECT TOP 1 @RoleId = Id FROM Roles WHERE Name = 'Admin' ORDER BY Id;

PRINT '=== İŞLEM YAPILIYOR ===';
PRINT 'Seçilen UserId: ' + CAST(ISNULL(@UserId, 0) AS VARCHAR);
PRINT 'Seçilen RoleId: ' + CAST(ISNULL(@RoleId, 0) AS VARCHAR);
PRINT '';

-- 3. İlişkiyi kontrol et ve ekle
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
        PRINT '✓ Admin kullanıcısına Admin rolü atandı.';
    END
    ELSE
    BEGIN
        PRINT '✓ Admin kullanıcısına Admin rolü zaten atanmış.';
    END
END
ELSE
BEGIN
    PRINT '✗ HATA: Admin kullanıcısı veya Admin rolü bulunamadı!';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'İşlem tamamlandı!';
PRINT '========================================';
GO

