-- =============================================
-- USER ROLES İLİŞKİSİNİ TEST ET
-- Login hatası için kontrol scripti
-- =============================================

USE SoruCevap;
GO

-- 1. Admin kullanıcısının Id'sini bul
DECLARE @AdminUserId INT;
SELECT TOP 1 @AdminUserId = Id FROM Users WHERE Username = 'admin' ORDER BY Id;

PRINT 'Admin UserId: ' + CAST(@AdminUserId AS VARCHAR);
PRINT '';

-- 2. Admin rolünün Id'sini bul
DECLARE @AdminRoleId INT;
SELECT TOP 1 @AdminRoleId = Id FROM Roles WHERE Name = 'Admin' ORDER BY Id;

PRINT 'Admin RoleId: ' + CAST(@AdminRoleId AS VARCHAR);
PRINT '';

-- 3. UserRoles ilişkisini kontrol et
PRINT '=== UserRoles İlişkileri ===';
SELECT 
    ur.UserId,
    ur.RoleId,
    u.Username,
    r.Name AS RoleName
FROM UserRoles ur
INNER JOIN Users u ON ur.UserId = u.Id
INNER JOIN Roles r ON ur.RoleId = r.Id
WHERE ur.UserId = @AdminUserId;
PRINT '';

-- 4. Eğer ilişki yoksa, ekle
IF NOT EXISTS (
    SELECT 1 FROM UserRoles 
    WHERE UserId = @AdminUserId 
    AND RoleId = @AdminRoleId
)
BEGIN
    PRINT 'İlişki bulunamadı! Ekleniyor...';
    
    -- UserRoles tablosunda Id kolonu YOK, sadece UserId ve RoleId ile ekle
    INSERT INTO UserRoles (UserId, RoleId, CreatedAt, UpdatedAt)
    VALUES (@AdminUserId, @AdminRoleId, GETDATE(), NULL);
    
    PRINT 'İlişki eklendi!';
END
ELSE
BEGIN
    PRINT 'İlişki mevcut.';
END
GO

PRINT '';
PRINT 'Test tamamlandı!';
GO

