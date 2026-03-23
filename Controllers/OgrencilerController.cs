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
        public OgrencilerController(KutuphaneContext context) => _context = context;

        // Arama Parametresi Eklendi
        public async Task<IActionResult> Index(string searchString)
        {
            var ogrenciler = _context.Ogrenciler.Include(o => o.Sinif).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                ogrenciler = ogrenciler.Where(s => s.Ad.Contains(searchString) || s.Soyad.Contains(searchString) || s.OkulNo.Contains(searchString));
                ViewData["CurrentFilter"] = searchString;
            }

            return View(await ogrenciler.ToListAsync());
        }

        // ÖĞRENCİ SİLME METODU
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var ogrenci = await _context.Ogrenciler.Include(o => o.OduncIslemleri).FirstOrDefaultAsync(o => o.Id == id);
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

        public IActionResult Create()
        {
            ViewBag.SinifId = new SelectList(_context.Siniflar.Select(s => new { Id = s.Id, Ad = s.Seviye + "-" + s.Sube }), "Id", "Ad");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Ogrenci ogrenci)
        {
            _context.Add(ogrenci);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}