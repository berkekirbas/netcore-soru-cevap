-- =============================================
-- DUPLICATE KAYITLARI TEMİZLEME (OPSİYONEL)
-- Sadece gereksiz duplicate kayıtları siler
-- =============================================

USE SoruCevap;
GO

PRINT '=== DUPLICATE TEMİZLEME ===';
PRINT '';

-- 1. Duplicate admin kullanıcılarını göster
PRINT 'Duplicate admin kullanıcıları:';
SELECT Id, Username, Email, CreatedAt 
FROM Users 
WHERE Username = 'admin' 
ORDER BY Id;
PRINT '';

-- 2. Duplicate admin rollerini göster
PRINT 'Duplicate admin rolleri:';
SELECT Id, Name, CreatedAt 
FROM Roles 
WHERE Name = 'Admin' 
ORDER BY Id;
PRINT '';

-- 3. İlk kayıtları koru, diğerlerini sil (OPSİYONEL - DİKKATLİ KULLANIN!)
-- Bu scripti çalıştırmadan önce yedek alın!

PRINT 'NOT: Aşağıdaki DELETE komutları yorum satırında.';
PRINT 'Eğer duplicate kayıtları silmek istiyorsanız, yorumları kaldırın.';
PRINT '';

/*
-- Duplicate admin kullanıcılarını sil (ilk kayıt hariç)
DELETE FROM Users 
WHERE Username = 'admin' 
AND Id NOT IN (SELECT MIN(Id) FROM Users WHERE Username = 'admin');
PRINT 'Duplicate admin kullanıcıları silindi.';
GO

-- Duplicate admin rollerini sil (ilk kayıt hariç)
DELETE FROM Roles 
WHERE Name = 'Admin' 
AND Id NOT IN (SELECT MIN(Id) FROM Roles WHERE Name = 'Admin');
PRINT 'Duplicate admin rolleri silindi.';
GO
*/

PRINT 'Temizleme işlemi tamamlandı (hiçbir şey silinmedi - yorum satırları aktif).';
GO

