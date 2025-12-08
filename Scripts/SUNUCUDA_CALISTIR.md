# Sunucuda SQL Script Çalıştırma

## 1. SQL Dosyasını Sunucuya Kopyalayın

`SetupDatabase_Server.sql` dosyasını sunucuya kopyalayın.

## 2. sqlcmd ile Çalıştırın

### Windows Sunucuda:

```cmd
sqlcmd -S SUNUCU_ADRESI -U sa -P -berke07- -d SoruCevap -i SetupDatabase_Server.sql
```

**Örnek (uzak sunucu):**
```cmd
sqlcmd -S 192.168.1.100,1433 -U sa -P -berke07- -d SoruCevap -i SetupDatabase_Server.sql
```

### Linux/macOS Sunucuda:

```bash
sqlcmd -S SUNUCU_ADRESI -U sa -P '-berke07-' -d SoruCevap -i SetupDatabase_Server.sql
```

**Örnek:**
```bash
sqlcmd -S 192.168.1.100,1433 -U sa -P '-berke07-' -d SoruCevap -i SetupDatabase_Server.sql
```

## 3. Alternatif: SQL Server Management Studio (SSMS) ile

1. SSMS'i açın
2. Sunucuya bağlanın
3. `SoruCevap` veritabanını seçin
4. `SetupDatabase_Server.sql` dosyasını açın
5. F5 ile çalıştırın

## 4. Veritabanı Adını Kontrol Edin

Eğer veritabanı adı farklıysa (örneğin `SoruCevapPortal`), script'teki `USE SoruCevap;` satırını değiştirin:

```sql
USE SoruCevapPortal;  -- veya hangi veritabanı adıysa
GO
```

## 5. Başarı Kontrolü

Script çalıştıktan sonra kontrol edin:

```sql
-- Admin kullanıcısını kontrol et
SELECT * FROM Users WHERE Username = 'admin';

-- Admin rolünü kontrol et
SELECT * FROM Roles WHERE Name = 'Admin';

-- UserRoles ilişkisini kontrol et
SELECT u.Username, r.Name AS RoleName
FROM UserRoles ur
INNER JOIN Users u ON ur.UserId = u.Id
INNER JOIN Roles r ON ur.RoleId = r.Id
WHERE u.Username = 'admin';
```

## Önemli Notlar:

- Şifre `-berke07-` (tire işaretleri dahil)
- Veritabanı adı: `SoruCevap` (appsettings.json'da görünen)
- Eğer migration'da farklı bir veritabanı adı kullanıldıysa, onu kullanın

