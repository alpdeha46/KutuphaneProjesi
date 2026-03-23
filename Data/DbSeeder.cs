using KutuphaneProjesi.Models;
using Microsoft.EntityFrameworkCore;

namespace KutuphaneProjesi.Data
{
    public static class DbSeeder
    {
        public static void Seed(KutuphaneContext context)
        {
            // Veritabanı bağlantısını kontrol et
            context.Database.EnsureCreated();

            // EĞER ESKİLERİ SİLMEK İSTİYORSAN BU SATIRLARI BİR KERELİK AKTİF ET:
            // context.OduncIslemleri.RemoveRange(context.OduncIslemleri);
            // context.Kitaplar.RemoveRange(context.Kitaplar);
            // context.Ogrenciler.RemoveRange(context.Ogrenciler);
            // context.SaveChanges();

            if (context.Kitaplar.Any()) return; 

            // 1. Kategoriler
            var kats = new List<Kategori> {
                new Kategori { Ad = "Dünya Klasikleri" },
                new Kategori { Ad = "Türk Edebiyatı" },
                new Kategori { Ad = "Bilim Kurgu" },
                new Kategori { Ad = "Tarih" },
                new Kategori { Ad = "Gelişim" }
            };
            context.Kategoriler.AddRange(kats);
            context.SaveChanges();

            // 2. Sınıflar
            var siniflar = new List<Sinif>();
            for (int i = 9; i <= 12; i++) {
                siniflar.Add(new Sinif { Seviye = i, Sube = "A" });
                siniflar.Add(new Sinif { Seviye = i, Sube = "B" });
            }
            context.Siniflar.AddRange(siniflar);
            context.SaveChanges();

            // 3. Gerçekçi Kitaplar
            var kitaplar = new List<(string Ad, string Yazar)> {
                ("Nutuk", "Mustafa Kemal Atatürk"), ("1984", "George Orwell"), ("Suç ve Ceza", "Dostoyevski"),
                ("Kürk Mantolu Madonna", "Sabahattin Ali"), ("Tutunamayanlar", "Oğuz Atay"), ("Simyacı", "Paulo Coelho"),
                ("Sefiller", "Victor Hugo"), ("Cesur Yeni Dünya", "Aldous Huxley"), ("İnce Memed", "Yaşar Kemal"),
                ("Küçük Prens", "Antoine de Saint-Exupéry"), ("Hayvan Çiftliği", "George Orwell"), ("Dönüşüm", "Franz Kafka"),
                ("Fahrenheit 451", "Ray Bradbury"), ("Çalıkuşu", "Reşat Nuri Güntekin"), ("Şeker Portakalı", "J.M. de Vasconcelos"),
                ("Savaş ve Barış", "Tolstoy"), ("Momo", "Michael Ende"), ("Körlük", "Jose Saramago"),
                ("Puslu Kıtalar Atlası", "İhsan Oktay Anar"), ("Serenad", "Zülfü Livaneli")
                // ... (İstediğin kadar ekleyebilirsin, döngüyle 50'ye tamamlarız)
            };

            Random rnd = new Random();
            foreach (var k in kitaplar) {
                context.Kitaplar.Add(new Kitap { 
                    Ad = k.Ad, Yazar = k.Yazar, StokAdedi = rnd.Next(3, 15), 
                    SayfaSayisi = rnd.Next(100, 800), KategoriId = kats[rnd.Next(kats.Count)].Id 
                });
            }

            // 4. Gerçekçi Öğrenciler
            string[] adlar = { "Ali", "Can", "Ece", "Deniz", "Mert", "Selin", "Burak", "Gamze", "Emir", "Melis" };
            string[] soyadlar = { "Yılmaz", "Kaya", "Demir", "Çetin", "Şahin", "Öztürk", "Bulut", "Yıldız" };

            for (int i = 0; i < 50; i++) {
                context.Ogrenciler.Add(new Ogrenci {
                    Ad = adlar[rnd.Next(adlar.Length)],
                    Soyad = soyadlar[rnd.Next(soyadlar.Length)],
                    OkulNo = (500 + i).ToString(),
                    SinifId = siniflar[rnd.Next(siniflar.Count)].Id
                });
            }

            context.SaveChanges();
        }
    }
}