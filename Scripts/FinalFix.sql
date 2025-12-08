-- =============================================
-- FİNAL DÜZELTME - UserRoles İlişkisi
-- Id kolonu OLMADAN çalışır
-- =============================================

USE SoruCevap;
GO

PRINT '=== FİNAL DÜZELTME ===';
PRINT '';

-- 1. Admin kullanıcısı ve rolünü bul
DECLARE @AdminUserId INT;
DECLARE @AdminRoleId INT;

SELECT TOP 1 @AdminUserId = Id FROM Users WHERE Username = 'admin' ORDER BY Id;
SELECT TOP 1 @AdminRoleId = Id FROM Roles WHERE Name = 'Admin' ORDER BY Id;

PRINT 'Admin UserId: ' + CAST(ISNULL(@AdminUserId, 0) AS VARCHAR);
PRINT 'Admin RoleId: ' + CAST(ISNULL(@AdminRoleId, 0) AS VARCHAR);
PRINT '';

-- 2. Mevcut ilişkiyi kontrol et
IF EXISTS (
    SELECT 1 FROM UserRoles 
    WHERE UserId = @AdminUserId 
    AND RoleId = @AdminRoleId
)
BEGIN
    PRINT '✓ İlişki zaten mevcut.';
    PRINT '';
    PRINT 'Mevcut ilişki:';
    SELECT 
        ur.UserId,
        ur.RoleId,
        u.Username,
        r.Name AS RoleName
    FROM UserRoles ur
    INNER JOIN Users u ON ur.UserId = u.Id
    INNER JOIN Roles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @AdminUserId AND ur.RoleId = @AdminRoleId;
END
ELSE
BEGIN
    PRINT '✗ İlişki bulunamadı! Ekleniyor...';
    
    -- İlişkiyi ekle (Id kolonu OLMADAN)
    INSERT INTO UserRoles (UserId, RoleId, CreatedAt, UpdatedAt)
    VALUES (@AdminUserId, @AdminRoleId, GETDATE(), NULL);
    
    PRINT '✓ İlişki eklendi!';
    PRINT '';
    PRINT 'Yeni ilişki:';
    SELECT 
        ur.UserId,
        ur.RoleId,
        u.Username,
        r.Name AS RoleName
    FROM UserRoles ur
    INNER JOIN Users u ON ur.UserId = u.Id
    INNER JOIN Roles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @AdminUserId AND ur.RoleId = @AdminRoleId;
END
GO

PRINT '';
PRINT '========================================';
PRINT 'İşlem tamamlandı!';
PRINT 'Şimdi projeyi çalıştırıp login yapabilirsiniz.';
PRINT '========================================';
GO

