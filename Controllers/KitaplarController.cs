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

        public KitaplarController(KutuphaneContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var kitaplar = _context.Kitaplar
                .Include(k => k.Kategori)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                kitaplar = kitaplar.Where(k =>
                    k.Ad.Contains(searchString) ||
                    (k.Yazar != null && k.Yazar.Contains(searchString)));

                ViewData["CurrentFilter"] = searchString;
            }

            return View(await kitaplar.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(_context.Kategoriler, "Id", "Ad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Kitap kitap)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.KategoriId = new SelectList(_context.Kategoriler, "Id", "Ad", kitap.KategoriId);
                return View(kitap);
            }

            _context.Kitaplar.Add(kitap);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Kitap eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kitap = await _context.Kitaplar
                .Include(k => k.OduncIslemleri)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kitap != null && !kitap.OduncIslemleri.Any(o => o.TeslimTarihi == null))
            {
                _context.Kitaplar.Remove(kitap);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Kitap silindi.";
            }
            else
            {
                TempData["Hata"] = "Kitap öğrencide olduğu için silinemez!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}