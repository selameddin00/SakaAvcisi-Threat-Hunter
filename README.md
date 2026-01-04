| Bilgi | Detay |
| :--- | :--- |
| **Ad Soyad** | Selameddin Tirit |
| **Öğrenci No** | 240541035 |
| **Bölüm** | Yazılım Mühendisliği (A) |
| **Fakülte** | Teknoloji Fakültesi |

# SakaAvcisi: İmza Tabanlı Tehdit Tespit ve Müdahale Aracı

## 📋 Giriş

**SakaAvcisi**, eğitim amaçlı geliştirilmiş bir Blue Team / Antivirus simülasyon aracıdır. Bu araç, "SakaVirusu" adlı eğitim amaçlı zararlı yazılım simülasyonunu tespit etmek, etkisiz hale getirmek ve sistem iyileştirmesi yapmak için tasarlanmıştır.

Araç, gerçek dünya siber güvenlik senaryolarında kullanılan **imza tabanlı tespit (signature-based detection)**, **olay müdahalesi (incident response)** ve **sistem kurtarma (system recovery)** tekniklerini simüle eder.

## ✨ Özellikler

### 🔍 İmza Tabanlı Tespit (Signature-Based Detection)

- **Binary Tarama:** Hedef dizindeki tüm `.exe` dosyalarını recursive olarak tarar
- **İmza Arama:** `X_KRAL_VIRUS_SIGNATURE_V1_X` imzasını hem UTF-8 hem de Unicode formatında arar
- **Metadata Analizi:** Dosya içeriğini binary ve text modda analiz eder
- **Güvenlik Kontrolü:** Kendi çalışan executable dosyasını (`FriendlyName`) tarama dışı bırakır

### ⚡ Olay Müdahalesi (Incident Response)

- **Süreç Sonlandırma:** Tespit edilen zararlı yazılımın çalışan sürecini (`Process.Kill`) sonlandırır
- **Dosya Silme:** Zararlı dosyayı diskten kalıcı olarak siler
- **Gerçek Zamanlı Raporlama:** Tespit ve müdahale işlemlerini renkli konsol çıktıları ile raporlar

### 🔧 Sistem İyileştirme (System Recovery)

- **Dosya Kurtarma:** `Enfected_` ön eki almış dosyaları tespit eder
- **Orijinal İsimlendirme:** Enfekte edilmiş dosyaları orijinal isimlerine geri döndürür
- **Çakışma Yönetimi:** Orijinal dosya zaten mevcutsa güvenli şekilde atlar

## 🚀 Kurulum ve Kullanım

### Gereksinimler

- .NET 8.0 SDK veya üzeri
- Windows işletim sistemi
- Yönetici yetkileri (süreç sonlandırma ve dosya silme işlemleri için)

### Derleme ve Yayınlama

Projeyi tek dosya (self-contained) executable olarak derlemek için:

```bash
# Proje dizinine gidin
cd SakaAvcisi

# Release modunda tek dosya olarak yayınlayın
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Yayınlanan dosya şu konumda bulunur:
```
bin/Release/net8.0/win-x64/publish/SakaAvcisi.exe
```

### Kullanım

1. **Yönetici Olarak Çalıştırma:**
   - `SakaAvcisi.exe` dosyasına sağ tıklayın
   - "Yönetici olarak çalıştır" seçeneğini seçin
   - Veya PowerShell/CMD'yi yönetici olarak açıp çalıştırın

2. **Tarama Başlatma:**
   - Program başladığında, taranacak dizin yolunu girin
   - Enter tuşuna basarak uygulama dizinini tarayabilirsiniz
   - Veya özel bir dizin yolu belirtebilirsiniz

3. **Sonuçları İnceleme:**
   - Tarama tamamlandığında, tespit edilen tehdit sayısı ve kurtarılan dosya sayısı raporlanır
   - Her işlem renkli konsol çıktıları ile gösterilir:
     - 🔴 **Kırmızı:** Tehdit tespit edildi
     - 🟢 **Yeşil:** Başarılı işlem
     - 🟡 **Sarı:** Uyarı mesajları

### Örnek Kullanım Senaryosu

```bash
# Yönetici PowerShell'de
.\SakaAvcisi.exe

# Konsol çıktısı:
# SAKA AVCISI v1.0
# 
# Taranacak dizin yolunu girin (Enter = Uygulama dizini): C:\Test
# 
# Tarama başlatılıyor: C:\Test
# 
# TEHDIT TESPIT EDILDI: C:\Test\malware.exe
# Calisan process sonlandiriliyor: malware (PID: 1234)
# Tehdit etkisiz hale getirildi: C:\Test\malware.exe
# Kurtarildi: Enfected_document.txt -> document.txt
# 
# === TARAMA TAMAMLANDI ===
# Toplam etkisiz hale getirilen tehdit sayisi: 1
# Toplam kurtarilan dosya sayisi: 1
```

## 🔗 İlişkili Proje

Bu aracı test etmek için kullanılan **Saldırı Simülasyonu (SakaVirusu)** projesine şuradan ulaşabilirsiniz: [LİNK GELECEK]

## ⚠️ Önemli Notlar

- Bu araç **eğitim amaçlı** geliştirilmiştir ve gerçek üretim ortamlarında kullanılmamalıdır
- Yönetici yetkileri gerektirir çünkü sistem süreçlerini sonlandırma ve dosya silme işlemleri yapar
- Sadece belirli bir imza (`X_KRAL_VIRUS_SIGNATURE_V1_X`) için tasarlanmıştır
- Kendi executable dosyasını tarama dışı bırakır, bu nedenle kendini silmez

## 🛠️ Teknik Detaylar

- **Platform:** .NET 8.0
- **Dil:** C#
- **Mimari:** Konsol Uygulaması
- **Tespit Yöntemi:** Binary imza tarama
- **Müdahale Yöntemi:** Process termination + File deletion
- **Kurtarma Yöntemi:** Dosya yeniden adlandırma

## 📝 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.

## 👤 Geliştirici

Kıdemli Olay Müdahale Uzmanı (Incident Responder) ve Yazılım Mimarı

---

**Uyarı:** Bu araç yalnızca eğitim ve araştırma amaçlıdır. Gerçek siber güvenlik senaryolarında profesyonel antivirus çözümleri kullanılmalıdır.

