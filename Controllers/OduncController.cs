using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Data;
using KutuphaneProjesi.Models;

namespace KutuphaneProjesi.Controllers
{
    public class OduncController : Controller
    {
        private readonly KutuphaneContext _context;
        public OduncController(KutuphaneContext context) => _context = context;

        public async Task<IActionResult> Index(string searchString)
        {
            var sorgu = _context.OduncIslemleri.Include(o => o.Kitap).Include(o => o.Ogrenci).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                sorgu = sorgu.Where(o => o.Kitap.Ad.Contains(searchString) || o.Ogrenci.Ad.Contains(searchString));
            }
            return View(await sorgu.OrderByDescending(o => o.VerilisTarihi).ToListAsync());
        }

        public IActionResult Create()
        {
            // PRO ARAMA: Kitap ve Yazar bilgisini birleştiriyoruz
            var kitaplar = _context.Kitaplar.Where(k => k.StokAdedi > 0).Select(k => new {
                Id = k.Id,
                Text = k.Ad + " - " + k.Yazar + " (Stok: " + k.StokAdedi + ")"
            }).ToList();

            // PRO ARAMA: Öğrenci Ad, Soyad ve Okul No bilgisini birleştiriyoruz
            var ogrenciler = _context.Ogrenciler.Select(o => new {
                Id = o.Id,
                Text = o.Ad + " " + o.Soyad + " [No: " + o.OkulNo + "]"
            }).ToList();

            ViewBag.KitapId = new SelectList(kitaplar, "Id", "Text");
            ViewBag.OgrenciId = new SelectList(ogrenciler, "Id", "Text");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OduncIslem odunc)
        {
            var kitap = await _context.Kitaplar.FindAsync(odunc.KitapId);
            if (kitap != null && kitap.StokAdedi > 0)
            {
                odunc.VerilisTarihi = DateTime.Now;
                _context.Add(odunc);
                kitap.StokAdedi--; 
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(odunc);
        }

        [HttpPost]
        public async Task<IActionResult> TeslimAl(int id)
        {
            var islem = await _context.OduncIslemleri.Include(o => o.Kitap).FirstOrDefaultAsync(o => o.Id == id);
            if (islem != null && islem.TeslimTarihi == null)
            {
                islem.TeslimTarihi = DateTime.Now;
                if (islem.Kitap != null) islem.Kitap.StokAdedi++;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}