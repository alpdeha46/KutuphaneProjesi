using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Data;
using KutuphaneProjesi.Models;

namespace KutuphaneProjesi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class KutuphaneApiController : ControllerBase
    {
        private readonly KutuphaneContext _context;

        public KutuphaneApiController(KutuphaneContext context)
        {
            _context = context;
        }

        [HttpGet("Kitaplar")]
        public async Task<IActionResult> GetKitaplar()
        {
            var data = await _context.Kitaplar
                .Include(k => k.Kategori)
                .Select(k => new
                {
                    k.Id,
                    k.Ad,
                    k.Yazar,
                    k.StokAdedi,
                    k.SayfaSayisi,
                    k.KategoriId,
                    Kategori = k.Kategori != null ? k.Kategori.Ad : ""
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("Ogrenciler")]
        public async Task<IActionResult> GetOgrenciler()
        {
            var data = await _context.Ogrenciler
                .Include(o => o.Sinif)
                .Select(o => new
                {
                    o.Id,
                    AdSoyad = o.Ad + " " + o.Soyad,
                    o.OkulNo,
                    o.SinifId,
                    Sinif = o.Sinif != null ? o.Sinif.Seviye + "-" + o.Sinif.Sube : ""
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("Kategoriler")]
        public async Task<IActionResult> GetKategoriler()
        {
            var data = await _context.Kategoriler
                .Select(c => new
                {
                    c.Id,
                    c.Ad,
                    KitapSayisi = c.Kitaplar.Count()
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("Siniflar")]
        public async Task<IActionResult> GetSiniflar()
        {
            var data = await _context.Siniflar
                .Select(s => new
                {
                    s.Id,
                    SinifAdi = s.Seviye + "-" + s.Sube,
                    OgrenciSayisi = s.Ogrenciler.Count(),
                    Ogrenciler = s.Ogrenciler.Select(o => o.Ad + " " + o.Soyad)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("Sinif/{id}")]
        public async Task<IActionResult> GetSinifDetail(int id)
        {
            var sinif = await _context.Siniflar
                .Include(s => s.Ogrenciler)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sinif == null)
                return NotFound(new { mesaj = "Sınıf bulunamadı!" });

            return Ok(new
            {
                sinif.Id,
                Ad = sinif.Seviye + "-" + sinif.Sube,
                Ogrenciler = sinif.Ogrenciler.Select(o => new
                {
                    o.Id,
                    o.Ad,
                    o.Soyad,
                    o.OkulNo
                })
            });
        }

        [HttpGet("Istatistikler")]
        public IActionResult GetIstatistikler()
        {
            var stats = new
            {
                kitapSayisi = _context.Kitaplar.Count(),
                ogrenciSayisi = _context.Ogrenciler.Count(),
                kategoriSayisi = _context.Kategoriler.Count(),
                sinifSayisi = _context.Siniflar.Count()
            };

            return Ok(stats);
        }

        [HttpPost("KitapEkle")]
        public async Task<IActionResult> KitapEkle([FromBody] KitapDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Ad))
                return BadRequest(new { mesaj = "Kitap adı boş olamaz" });

            var kategoriVar = await _context.Kategoriler.AnyAsync(x => x.Id == dto.KategoriId);
            if (!kategoriVar)
                return BadRequest(new { mesaj = "Geçerli kategori seçilmedi" });

            var kitap = new Kitap
            {
                Ad = dto.Ad.Trim(),
                Yazar = dto.Yazar?.Trim(),
                StokAdedi = dto.StokAdedi,
                SayfaSayisi = dto.SayfaSayisi,
                KategoriId = dto.KategoriId
            };

            _context.Kitaplar.Add(kitap);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Kitap eklendi" });
        }

        [HttpPost("OgrenciEkle")]
        public async Task<IActionResult> OgrenciEkle([FromBody] OgrenciDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.AdSoyad))
                return BadRequest(new { mesaj = "Ad Soyad boş olamaz" });

            var sinifVar = await _context.Siniflar.AnyAsync(x => x.Id == dto.SinifId);
            if (!sinifVar)
                return BadRequest(new { mesaj = "Geçerli sınıf seçilmedi" });

            var parcalar = dto.AdSoyad.Trim().Split(' ', 2);
            var ad = parcalar[0];
            var soyad = parcalar.Length > 1 ? parcalar[1] : "";

            var ogrenci = new Ogrenci
            {
                Ad = ad,
                Soyad = soyad,
                OkulNo = dto.OkulNo.Trim(),
                SinifId = dto.SinifId
            };

            _context.Ogrenciler.Add(ogrenci);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Öğrenci eklendi" });
        }

        [HttpPost("KategoriEkle")]
        public async Task<IActionResult> KategoriEkle([FromBody] KategoriDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Ad))
                return BadRequest(new { mesaj = "Kategori adı boş olamaz" });

            var kategori = new Kategori
            {
                Ad = dto.Ad.Trim()
            };

            _context.Kategoriler.Add(kategori);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Kategori eklendi" });
        }

        [HttpPost("SinifEkle")]
        public async Task<IActionResult> SinifEkle([FromBody] SinifDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SinifAdi))
                return BadRequest(new { mesaj = "Sınıf adı boş olamaz" });

            var parcalar = dto.SinifAdi.Trim().Split('-', 2);

            if (parcalar.Length < 2)
                return BadRequest(new { mesaj = "Sınıf formatı 10-A gibi olmalı" });

            if (!int.TryParse(parcalar[0], out int seviye))
                return BadRequest(new { mesaj = "Sınıf seviyesi sayısal olmalı" });

            var temizSube = parcalar[1].Trim().ToUpper();

            var varMi = await _context.Siniflar.AnyAsync(s => s.Seviye == seviye && s.Sube == temizSube);
            if (varMi)
                return BadRequest(new { mesaj = "Bu sınıf zaten mevcut" });

            var sinif = new Sinif
            {
                Seviye = seviye,
                Sube = temizSube
            };

            _context.Siniflar.Add(sinif);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Sınıf eklendi" });
        }

        [HttpPut("KitapGuncelle/{id}")]
        public async Task<IActionResult> KitapGuncelle(int id, [FromBody] KitapDto dto)
        {
            var kitap = await _context.Kitaplar.FindAsync(id);
            if (kitap == null)
                return NotFound(new { mesaj = "Kitap bulunamadı" });

            if (string.IsNullOrWhiteSpace(dto.Ad))
                return BadRequest(new { mesaj = "Kitap adı boş olamaz" });

            var kategoriVar = await _context.Kategoriler.AnyAsync(x => x.Id == dto.KategoriId);
            if (!kategoriVar)
                return BadRequest(new { mesaj = "Geçerli kategori seçilmedi" });

            kitap.Ad = dto.Ad.Trim();
            kitap.Yazar = dto.Yazar?.Trim();
            kitap.StokAdedi = dto.StokAdedi;
            kitap.SayfaSayisi = dto.SayfaSayisi;
            kitap.KategoriId = dto.KategoriId;

            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Kitap güncellendi" });
        }

        [HttpPut("OgrenciGuncelle/{id}")]
        public async Task<IActionResult> OgrenciGuncelle(int id, [FromBody] OgrenciDto dto)
        {
            var ogrenci = await _context.Ogrenciler.FindAsync(id);
            if (ogrenci == null)
                return NotFound(new { mesaj = "Öğrenci bulunamadı" });

            if (string.IsNullOrWhiteSpace(dto.AdSoyad))
                return BadRequest(new { mesaj = "Ad Soyad boş olamaz" });

            var sinifVar = await _context.Siniflar.AnyAsync(x => x.Id == dto.SinifId);
            if (!sinifVar)
                return BadRequest(new { mesaj = "Geçerli sınıf seçilmedi" });

            var parcalar = dto.AdSoyad.Trim().Split(' ', 2);

            ogrenci.Ad = parcalar[0];
            ogrenci.Soyad = parcalar.Length > 1 ? parcalar[1] : "";
            ogrenci.OkulNo = dto.OkulNo.Trim();
            ogrenci.SinifId = dto.SinifId;

            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Öğrenci güncellendi" });
        }

        [HttpPut("KategoriGuncelle/{id}")]
        public async Task<IActionResult> KategoriGuncelle(int id, [FromBody] KategoriDto dto)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori == null)
                return NotFound(new { mesaj = "Kategori bulunamadı" });

            if (string.IsNullOrWhiteSpace(dto.Ad))
                return BadRequest(new { mesaj = "Kategori adı boş olamaz" });

            kategori.Ad = dto.Ad.Trim();
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Kategori güncellendi" });
        }

        [HttpPut("SinifGuncelle/{id}")]
        public async Task<IActionResult> SinifGuncelle(int id, [FromBody] SinifDto dto)
        {
            var sinif = await _context.Siniflar.FindAsync(id);
            if (sinif == null)
                return NotFound(new { mesaj = "Sınıf bulunamadı" });

            if (string.IsNullOrWhiteSpace(dto.SinifAdi))
                return BadRequest(new { mesaj = "Sınıf adı boş olamaz" });

            var parcalar = dto.SinifAdi.Trim().Split('-', 2);

            if (parcalar.Length < 2)
                return BadRequest(new { mesaj = "Sınıf formatı 10-A gibi olmalı" });

            if (!int.TryParse(parcalar[0], out int seviye))
                return BadRequest(new { mesaj = "Sınıf seviyesi sayısal olmalı" });

            sinif.Seviye = seviye;
            sinif.Sube = parcalar[1].Trim().ToUpper();

            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Sınıf güncellendi" });
        }

        [HttpDelete("KitapSil/{id}")]
        public async Task<IActionResult> KitapSil(int id)
        {
            var kitap = await _context.Kitaplar
                .Include(k => k.OduncIslemleri)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kitap == null)
                return NotFound(new { mesaj = "Kitap bulunamadı" });

            if (kitap.OduncIslemleri.Any(o => o.TeslimTarihi == null))
                return BadRequest(new { mesaj = "Kitap öğrencide olduğu için silinemez" });

            _context.Kitaplar.Remove(kitap);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Kitap silindi" });
        }

        [HttpDelete("OgrenciSil/{id}")]
        public async Task<IActionResult> OgrenciSil(int id)
        {
            var ogrenci = await _context.Ogrenciler
                .Include(o => o.OduncIslemleri)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ogrenci == null)
                return NotFound(new { mesaj = "Öğrenci bulunamadı" });

            if (ogrenci.OduncIslemleri.Any(o => o.TeslimTarihi == null))
                return BadRequest(new { mesaj = "Öğrencinin üzerinde kitap var, silinemez" });

            _context.Ogrenciler.Remove(ogrenci);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Öğrenci silindi" });
        }

        [HttpDelete("KategoriSil/{id}")]
        public async Task<IActionResult> KategoriSil(int id)
        {
            var kategori = await _context.Kategoriler
                .Include(k => k.Kitaplar)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kategori == null)
                return NotFound(new { mesaj = "Kategori bulunamadı" });

            if (kategori.Kitaplar.Any())
                return BadRequest(new { mesaj = "Bu kategoriye ait kitaplar var, silinemez" });

            _context.Kategoriler.Remove(kategori);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Kategori silindi" });
        }

        [HttpDelete("SinifSil/{id}")]
        public async Task<IActionResult> SinifSil(int id)
        {
            var sinif = await _context.Siniflar
                .Include(s => s.Ogrenciler)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sinif == null)
                return NotFound(new { mesaj = "Sınıf bulunamadı" });

            if (sinif.Ogrenciler.Any())
                return BadRequest(new { mesaj = "Sınıf dolu olduğu için silinemez" });

            _context.Siniflar.Remove(sinif);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Sınıf silindi" });
        }
    }
}