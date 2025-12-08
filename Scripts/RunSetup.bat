@echo off
REM Soru Cevap Portal - Veritabanı Kurulum Scripti
REM Windows için

echo ==========================================
echo Soru Cevap Portal - Veritabanı Kurulumu
echo ==========================================
echo.

REM Connection bilgileri (appsettings.json'dan)
set SERVER=localhost,1433
set USER=sa
set PASSWORD=-berke07-
set DATABASE=SoruCevap

echo Sunucu: %SERVER%
echo Veritabanı: %DATABASE%
echo Kullanıcı: %USER%
echo.
echo SQL script çalıştırılıyor...
echo.

REM SQL dosyasını çalıştır
sqlcmd -S %SERVER% -U %USER% -P %PASSWORD% -d %DATABASE% -i SetupDatabase.sql

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ==========================================
    echo Kurulum başarıyla tamamlandı!
    echo ==========================================
) else (
    echo.
    echo ==========================================
    echo HATA: Kurulum sırasında bir sorun oluştu!
    echo ==========================================
)

pause

