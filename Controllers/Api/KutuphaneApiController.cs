using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Data;

namespace KutuphaneProjesi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class KutuphaneApiController : ControllerBase
    {
        private readonly KutuphaneContext _context;
        public KutuphaneApiController(KutuphaneContext context) => _context = context;

        // 1. KİTAPLAR: GET /api/KutuphaneApi/Kitaplar
        [HttpGet("Kitaplar")]
        public async Task<IActionResult> GetKitaplar()
        {
            var data = await _context.Kitaplar.Include(k => k.Kategori)
                .Select(k => new { k.Id, k.Ad, k.Yazar, Kategori = k.Kategori.Ad, k.StokAdedi })
                .ToListAsync();
            return Ok(data);
        }

        // 2. ÖĞRENCİLER: GET /api/KutuphaneApi/Ogrenciler
        [HttpGet("Ogrenciler")]
        public async Task<IActionResult> GetOgrenciler()
        {
            var data = await _context.Ogrenciler.Include(o => o.Sinif)
                .Select(o => new { o.Id, AdSoyad = o.Ad + " " + o.Soyad, o.OkulNo, Sinif = o.Sinif.Seviye + "-" + o.Sinif.Sube })
                .ToListAsync();
            return Ok(data);
        }

        // 3. KATEGORİLER: GET /api/KutuphaneApi/Kategoriler
        [HttpGet("Kategoriler")]
        public async Task<IActionResult> GetKategoriler()
        {
            var data = await _context.Kategoriler
                .Select(c => new { c.Id, c.Ad, KitapSayisi = c.Kitaplar.Count() })
                .ToListAsync();
            return Ok(data);
        }

        // 4. SINIFLAR (YENİ): GET /api/KutuphaneApi/Siniflar
        [HttpGet("Siniflar")]
        public async Task<IActionResult> GetSiniflar()
        {
            var data = await _context.Siniflar
                .Select(s => new {
                    s.Id,
                    SinifAdi = s.Seviye + "-" + s.Sube,
                    OgrenciSayisi = s.Ogrenciler.Count(),
                    // Sınıftaki öğrencilerin listesini de istersen:
                    Ogrenciler = s.Ogrenciler.Select(o => o.Ad + " " + o.Soyad)
                }).ToListAsync();
            return Ok(data);
        }

        // 5. SINIF DETAY (ID İLE): GET /api/KutuphaneApi/Sinif/1
        [HttpGet("Sinif/{id}")]
        public async Task<IActionResult> GetSinifDetail(int id)
        {
            var sinif = await _context.Siniflar
                .Include(s => s.Ogrenciler)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sinif == null) return NotFound(new { Mesaj = "Sınıf bulunamadı!" });

            return Ok(new {
                Id = sinif.Id,
                Ad = sinif.Seviye + "-" + sinif.Sube,
                Ogrenciler = sinif.Ogrenciler.Select(o => new { o.Id, o.Ad, o.Soyad, o.OkulNo })
            });
        }
    }
}