using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class Kategori
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [Display(Name = "Kategori Adı")]
        public string Ad { get; set; }

        // Bir kategoride birden fazla kitap olabilir (İlişki)
        public List<Kitap> Kitaplar { get; set; }
    }
}