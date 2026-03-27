using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Data;
using KutuphaneProjesi.Models;

namespace KutuphaneProjesi.Controllers
{
    public class KategorilerController : Controller
    {
        private readonly KutuphaneContext _context;

        public KategorilerController(KutuphaneContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Kategoriler
                .Include(k => k.Kitaplar)
                .ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Kategori kategori)
        {
            if (string.IsNullOrWhiteSpace(kategori.Ad))
            {
                TempData["Hata"] = "Kategori adı boş olamaz.";
                return RedirectToAction(nameof(Index));
            }

            kategori.Ad = kategori.Ad.Trim();

            bool varMi = await _context.Kategoriler.AnyAsync(k => k.Ad == kategori.Ad);
            if (varMi)
            {
                TempData["Hata"] = "Bu kategori zaten mevcut.";
                return RedirectToAction(nameof(Index));
            }

            _context.Kategoriler.Add(kategori);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Kategori eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kategori = await _context.Kategoriler
                .Include(k => k.Kitaplar)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kategori == null)
            {
                TempData["Hata"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (kategori.Kitaplar.Any())
            {
                TempData["Hata"] = "Bu kategoriye ait kitaplar olduğu için silinemez.";
                return RedirectToAction(nameof(Index));
            }

            _context.Kategoriler.Remove(kategori);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Kategori silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}