-- =============================================
-- Veritabanı Kontrol Scripti
-- Bu scripti çalıştırarak tabloların oluşup oluşmadığını kontrol edin
-- =============================================

-- Tüm tabloları listele
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Her tablonun kolonlarını kontrol et
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN ('Users', 'Roles', 'UserRoles', 'Categories', 'Questions', 'Answers', 'QuestionVotes')
ORDER BY TABLE_NAME, ORDINAL_POSITION;

-- Tablo sayılarını kontrol et
SELECT 
    'Users' AS TableName, COUNT(*) AS RowCount FROM Users
UNION ALL
SELECT 'Roles', COUNT(*) FROM Roles
UNION ALL
SELECT 'UserRoles', COUNT(*) FROM UserRoles
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'Questions', COUNT(*) FROM Questions
UNION ALL
SELECT 'Answers', COUNT(*) FROM Answers
UNION ALL
SELECT 'QuestionVotes', COUNT(*) FROM QuestionVotes;

