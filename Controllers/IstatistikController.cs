using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Data;

namespace KutuphaneProjesi.Controllers
{
    public class IstatistikController : Controller
    {
        private readonly KutuphaneContext _context;
        public IstatistikController(KutuphaneContext context) => _context = context;

        public IActionResult Index()
        {
            ViewBag.KitapSayisi = _context.Kitaplar.Sum(k => k.StokAdedi); // Toplam fiziksel kitap sayısı
            ViewBag.OgrenciSayisi = _context.Ogrenciler.Count();
            ViewBag.AktifOduncSayisi = _context.OduncIslemleri.Count(o => o.TeslimTarihi == null); // Şu an öğrencide olanlar
            return View();
        }
    }
}