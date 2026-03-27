using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Data;
using KutuphaneProjesi.Models;

namespace KutuphaneProjesi.Controllers
{
    public class OgrencilerController : Controller
    {
        private readonly KutuphaneContext _context;

        public OgrencilerController(KutuphaneContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var ogrenciler = _context.Ogrenciler
                .Include(o => o.Sinif)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                ogrenciler = ogrenciler.Where(s =>
                    s.Ad.Contains(searchString) ||
                    s.Soyad.Contains(searchString) ||
                    s.OkulNo.Contains(searchString));

                ViewData["CurrentFilter"] = searchString;
            }

            return View(await ogrenciler.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.SinifId = new SelectList(
                _context.Siniflar.Select(s => new
                {
                    Id = s.Id,
                    Ad = s.Seviye + "-" + s.Sube
                }).ToList(),
                "Id",
                "Ad"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ogrenci ogrenci)
        {
            if (string.IsNullOrWhiteSpace(ogrenci.Ad) ||
                string.IsNullOrWhiteSpace(ogrenci.Soyad) ||
                string.IsNullOrWhiteSpace(ogrenci.OkulNo) ||
                ogrenci.SinifId <= 0)
            {
                TempData["Hata"] = "Tüm alanları doldurun.";

                ViewBag.SinifId = new SelectList(
                    _context.Siniflar.Select(s => new
                    {
                        Id = s.Id,
                        Ad = s.Seviye + "-" + s.Sube
                    }).ToList(),
                    "Id",
                    "Ad",
                    ogrenci.SinifId
                );

                return View(ogrenci);
            }

            bool okulNoVarMi = await _context.Ogrenciler.AnyAsync(o => o.OkulNo == ogrenci.OkulNo);
            if (okulNoVarMi)
            {
                TempData["Hata"] = "Bu okul numarası zaten kayıtlı.";

                ViewBag.SinifId = new SelectList(
                    _context.Siniflar.Select(s => new
                    {
                        Id = s.Id,
                        Ad = s.Seviye + "-" + s.Sube
                    }).ToList(),
                    "Id",
                    "Ad",
                    ogrenci.SinifId
                );

                return View(ogrenci);
            }

            _context.Ogrenciler.Add(ogrenci);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Öğrenci eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ogrenci = await _context.Ogrenciler
                .Include(o => o.OduncIslemleri)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ogrenci != null)
            {
                if (ogrenci.OduncIslemleri.Any(o => o.TeslimTarihi == null))
                {
                    TempData["Hata"] = "Öğrencinin üzerinde kitap var, silinemez!";
                }
                else
                {
                    _context.Ogrenciler.Remove(ogrenci);
                    await _context.SaveChangesAsync();
                    TempData["Mesaj"] = "Öğrenci başarıyla silindi.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}