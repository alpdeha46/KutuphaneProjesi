using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class Sinif
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Seviye 1 ile 12 arasında olmalıdır.")]
        [Display(Name = "Sınıf Seviyesi (Yıl)")]
        public int Seviye { get; set; } // Örn: 1, 2, 3... 12

        [Required]
        [StringLength(5)]
        [Display(Name = "Şube")]
        public string Sube { get; set; } // Örn: A, B, C, Fen-A

        // Tam Adı otomatik birleştiren özellik (Örn: 11-B)
        public string TamAd => $"{Seviye}-{Sube}";

        public List<Ogrenci> Ogrenciler { get; set; }
    }
}