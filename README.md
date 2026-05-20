# Muhasebe

Muhasebe, kurum ici finans ve masraf sureclerini tek bir platformda yonetmek icin tasarlanmis rol bazli bir web uygulamasidir.  
Bu proje; personelden yonetime, finans ekibinden sistem yoneticisine kadar farkli kullanici tiplerinin ihtiyac duydugu ekranlari tek bir cati altinda toplar.

## Projenin Vizyonu

Tamamlanmis halinde Muhasebe uygulamasi:

- Masraf taleplerinin olusturulmasi, takip edilmesi ve sonuclandirilmasi
- Cok adimli onay mekanizmasinin yonetilmesi
- Finansal hareketlerin merkezi bir ekranda izlenmesi
- Departman bazli butce kontrolunun saglanmasi
- Rol bazli yetkilendirme ile guvenli erisim modeli

sunmayi hedefler.

## Cozdgu Is Problemleri

- E-posta/Excel uzerinden daginik ilerleyen talep sureclerini tek merkeze toplar.
- Kimin hangi adimda onay verecegini netlestirir.
- Harcama ve butce hareketlerinde izlenebilirlik saglar.
- Rol disi ekranlara erisimi engelleyerek operasyonel guvenligi artirir.

## Tamamlanmis Urunde Baslica Moduller

### 1) Kimlik ve Erisim Yonetimi

- Giris, kayit, cikis akisleri
- Rol bazli yetki kontrolu
- Kullanici bazli menu ve ekran gorunurlugu

### 2) Panel ve Ozet Ekrani

- Kuruma ait genel finansal durum ozeti
- Bekleyen talepler, kritik bildirimler ve metrik kartlari
- Role gore ozellestirilmis kisa yol alanlari

### 3) Masraf Talep Yonetimi

- Yeni talep olusturma
- Talep durum takibi (bekliyor, onaylandi, reddedildi vb.)
- Talep gecmisi ve aciklama kayitlari

### 4) Onay Sureci Yonetimi

- Yetkili kullanicilar icin bekleyen is listesi
- Onay / red islemleri ve red gerekcesi kaydi
- Onay adimlarinin raporlanabilir gecmisi

### 5) Finans ve Kasa Yonetimi

- Gelir-gider hareketlerinin izlenmesi
- Kasa gorunumu ve donemsel finans raporlari
- Talep-finans iliskisinin takip edilmesi

### 6) Departman ve Butce Yonetimi

- Departman bazli butce tanimlari
- Ayrilan ve kullanilan butce takibi
- Departman sorumlulari icin ekip gorunumu

### 7) Yonetim Modulu

- Rol ve menu eslestirmeleri
- Kullanici/rol iliskilerinin yonetimi
- Uygulama genel ayar ve yetki parametreleri

## Rol Bazli Deneyim

Tamamlanmis yapida kullanici arayuzu, kullanicinin rolune gore otomatik sekillenir:

- **Personel:** Kendi taleplerini olusturur ve takip eder.
- **Mudur / Yonetici:** Kendisine dusen talepleri degerlendirir, ekip tarafini gorur.
- **Finans:** Finansal islem ve kasa ekranlarina odaklanir.
- **Admin:** Tum modullere erisir, rol/yetki konfigurasyonunu yonetir.

## Teknik Altyapi (Hedef Mimari)

- ASP.NET Core MVC tabanli katmanli yapi
- Entity Framework Core ile SQL Server veri erisimi
- Razor tabanli dinamik ve rol bazli ekranlar
- `wwwroot` altinda modern, yeniden kullanilabilir frontend varliklari

## Beklenen Kazanim

Muhasebe uygulamasi tamamlandiginda;

- Surecler hizlanir,
- Onay adimlari standardize olur,
- Finansal hareketler daha seffaf hale gelir,
- Kurum ici operasyonel kontrol ve raporlanabilirlik guclenir.

