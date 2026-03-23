using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Data;
using KutuphaneProjesi.Models;

namespace KutuphaneProjesi.Controllers
{
    public class KitaplarController : Controller
    {
        private readonly KutuphaneContext _context;
        public KitaplarController(KutuphaneContext context) => _context = context;

        // Arama Parametresi (searchString) eklendi
        public async Task<IActionResult> Index(string searchString)
        {
            var kitaplar = _context.Kitaplar.Include(k => k.Kategori).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                // Hem kitap adında hem de yazar adında arama yapar
                kitaplar = kitaplar.Where(k => k.Ad.Contains(searchString) || k.Yazar.Contains(searchString));
                ViewData["CurrentFilter"] = searchString; // Arama kutusunda kelime kalsın diye
            }

            return View(await kitaplar.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(_context.Kategoriler, "Id", "Ad");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Kitap kitap)
        {
            _context.Add(kitap);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var kitap = await _context.Kitaplar.Include(k => k.OduncIslemleri).FirstOrDefaultAsync(k => k.Id == id);
            if (kitap != null && !kitap.OduncIslemleri.Any(o => o.TeslimTarihi == null))
            {
                _context.Kitaplar.Remove(kitap);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Kitap silindi.";
            }
            else TempData["Hata"] = "Kitap öğrencide olduğu için silinemez!";
            
            return RedirectToAction(nameof(Index));
        }
    }
}