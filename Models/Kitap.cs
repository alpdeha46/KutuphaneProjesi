using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class Kitap
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kitap adı zorunludur")]
        public string Ad { get; set; }

        public string? Yazar { get; set; }
        
        [Range(0, 1000, ErrorMessage = "Stok 0-1000 arası olmalıdır")]
        public int StokAdedi { get; set; } // YENİ EKLENEN ALAN

        public int SayfaSayisi { get; set; }

        public int KategoriId { get; set; }
        public Kategori? Kategori { get; set; }

        public ICollection<OduncIslem> OduncIslemleri { get; set; } = new List<OduncIslem>();
    }
}