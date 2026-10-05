# Vertigo Games - Wheel of Fortune Case

Unity ile hazırladığım çarkıfelek (Wheel of Fortune) UI case çalışması.

## Build & Dosyalar
* **APK & Demo Video:** Çalıştırılabilir Android APK dosyasına (`vertigo-case.apk`), oynanış videosuna ve farklı ekran oranlarına ait ekran görüntülerine [Releases (v1.0.0)](https://github.com/armacx/vrtg/releases/tag/v1.0.0) sekmesinden ulaşabilirsiniz.

## Proje Hakkında Notlar
* **Mimari:** Kodları MVC mantığında tutmaya özen gösterdim; veri, arayüz ve akış kontrolleri birbirinden bağımsız çalışıyor.
* **Haberleşme:** Sınıflar arasındaki bağlantıyı C# event/action yapılarıyla kurarak bileşenlerin birbirine sıkı sıkıya bağlanmasını (tight coupling) engelledim.
* **Konfigürasyon:** Çark dilimleri, safe zone mantığı ve ödül havuzu `ScriptableObject` üzerinden kolayca yönetilebiliyor.
* **UI & Layout:** Farklı telefon ekranlarına uyum sağlaması için canvas yapısı esnek tutuldu (16:9, 20:9 ve 4:3 oranlarında test edildi). UI referansları editör tarafında `OnValidate` ile otomatik bağlanıyor.
