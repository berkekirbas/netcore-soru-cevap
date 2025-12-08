# SQLCMD ile SQL Dosyası Çalıştırma

## Windows'ta (Command Prompt veya PowerShell)

```cmd
sqlcmd -S SUNUCU_ADRESI -U KULLANICI_ADI -P SIFRE -d SoruCevapPortal -i Scripts\SetupDatabase.sql
```

**Örnek:**
```cmd
sqlcmd -S 192.168.1.100,1433 -U sa -P MyPassword123 -d SoruCevapPortal -i Scripts\SetupDatabase.sql
```

## macOS/Linux'ta

```bash
sqlcmd -S SUNUCU_ADRESI -U KULLANICI_ADI -P SIFRE -d SoruCevapPortal -i Scripts/SetupDatabase.sql
```

**Örnek:**
```bash
sqlcmd -S 192.168.1.100,1433 -U sa -P MyPassword123 -d SoruCevapPortal -i Scripts/SetupDatabase.sql
```

## Parametreler:
- `-S`: Sunucu adresi (IP veya hostname)
- `-U`: Kullanıcı adı
- `-P`: Şifre
- `-d`: Veritabanı adı
- `-i`: Çalıştırılacak SQL dosyası yolu

## Alternatif: Şifreyi komut satırında yazmak istemiyorsanız

```bash
sqlcmd -S SUNUCU_ADRESI -U KULLANICI_ADI -d SoruCevapPortal -i Scripts/SetupDatabase.sql
```
Bu komut şifreyi interaktif olarak sorar.

## SQL Server Authentication yerine Windows Authentication kullanıyorsanız

```bash
sqlcmd -S SUNUCU_ADRESI -E -d SoruCevapPortal -i Scripts/SetupDatabase.sql
```

## Çıktıyı dosyaya kaydetmek için

```bash
sqlcmd -S SUNUCU_ADRESI -U KULLANICI_ADI -P SIFRE -d SoruCevapPortal -i Scripts/SetupDatabase.sql -o output.txt
```

