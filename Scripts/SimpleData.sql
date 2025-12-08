-- =============================================
-- BASİT VERİ EKLEME - Adım Adım
-- =============================================

-- Adım 1: Admin Rolü
INSERT INTO Roles (Name, CreatedAt, UpdatedAt) 
VALUES ('Admin', GETDATE(), NULL);
GO

-- Adım 2: Admin Kullanıcısı
INSERT INTO Users (Username, Email, Password, FullName, IsActive, CreatedAt, UpdatedAt)
VALUES ('admin', 'admin@example.com', 'admin123', 'Admin User', 1, GETDATE(), NULL);
GO

-- Adım 3: UserRoles için Id değeri hesapla ve ekle
DECLARE @UserId INT = (SELECT Id FROM Users WHERE Username = 'admin');
DECLARE @RoleId INT = (SELECT Id FROM Roles WHERE Name = 'Admin');
DECLARE @NewId INT = ISNULL((SELECT MAX(Id) FROM UserRoles), 0) + 1;

INSERT INTO UserRoles (UserId, RoleId, Id, CreatedAt, UpdatedAt)
VALUES (@UserId, @RoleId, @NewId, GETDATE(), NULL);
GO

PRINT 'İşlem tamamlandı!';
PRINT 'Kullanıcı Adı: admin';
PRINT 'Şifre: admin123';
GO

