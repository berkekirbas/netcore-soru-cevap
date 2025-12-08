-- =============================================
-- UserRoles Tablosu Yapısını Kontrol Et
-- =============================================

-- UserRoles tablosunun kolonlarını göster
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'UserRoles'
ORDER BY ORDINAL_POSITION;

