using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Data;
using KutuphaneProjesi.Models;

namespace KutuphaneProjesi.Controllers
{
    public class SiniflarController : Controller
    {
        private readonly KutuphaneContext _context;

        public SiniflarController(KutuphaneContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Siniflar.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Seviye, string Sube)
        {
            if (Seviye >= 1 && !string.IsNullOrWhiteSpace(Sube))
            {
                string temizSube = Sube.Trim().ToUpper();

                bool varMi = await _context.Siniflar.AnyAsync(s =>
                    s.Seviye == Seviye && s.Sube == temizSube);

                if (varMi)
                {
                    TempData["Hata"] = $"{Seviye}-{temizSube} sınıfı zaten var!";
                }
                else
                {
                    _context.Siniflar.Add(new Sinif
                    {
                        Seviye = Seviye,
                        Sube = temizSube
                    });

                    await _context.SaveChangesAsync();
                    TempData["Mesaj"] = "Sınıf eklendi.";
                }
            }
            else
            {
                TempData["Hata"] = "Geçerli sınıf bilgisi giriniz.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SinifAtlat()
        {
            var tumSiniflar = await _context.Siniflar.ToListAsync();

            if (tumSiniflar.Any())
            {
                foreach (var sinif in tumSiniflar)
                {
                    if (sinif.Seviye < 12)
                    {
                        sinif.Seviye += 1;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "İşlem Başarılı: Tüm sınıflar bir üst seviyeye aktarıldı!";
            }
            else
            {
                TempData["Hata"] = "Sistemde atlatılacak sınıf bulunamadı!";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sinif = await _context.Siniflar
                .Include(s => s.Ogrenciler)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sinif != null && !sinif.Ogrenciler.Any())
            {
                _context.Siniflar.Remove(sinif);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Sınıf silindi.";
            }
            else
            {
                TempData["Hata"] = "Sınıf dolu veya bulunamadı.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}