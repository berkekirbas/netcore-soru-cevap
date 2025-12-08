#!/bin/bash
# Soru Cevap Portal - Veritabanı Kurulum Scripti
# macOS/Linux için

echo "=========================================="
echo "Soru Cevap Portal - Veritabanı Kurulumu"
echo "=========================================="
echo ""

# Connection bilgileri (appsettings.json'dan)
SERVER="localhost,1433"
USER="sa"
PASSWORD="-berke07-"
DATABASE="SoruCevap"

echo "Sunucu: $SERVER"
echo "Veritabanı: $DATABASE"
echo "Kullanıcı: $USER"
echo ""
echo "SQL script çalıştırılıyor..."
echo ""

# SQL dosyasını çalıştır
sqlcmd -S "$SERVER" -U "$USER" -P "$PASSWORD" -d "$DATABASE" -i SetupDatabase.sql

if [ $? -eq 0 ]; then
    echo ""
    echo "=========================================="
    echo "Kurulum başarıyla tamamlandı!"
    echo "=========================================="
else
    echo ""
    echo "=========================================="
    echo "HATA: Kurulum sırasında bir sorun oluştu!"
    echo "=========================================="
fi

