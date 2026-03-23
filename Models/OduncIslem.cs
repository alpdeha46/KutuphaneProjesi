using System;
using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class OduncIslem
    {
        public int Id { get; set; }

        [Required]
        public int KitapId { get; set; }
        public Kitap? Kitap { get; set; }

        [Required]
        public int OgrenciId { get; set; }
        public Ogrenci? Ogrenci { get; set; }

        public DateTime VerilisTarihi { get; set; }
        
        // Hata buradaydı: İsmi sabitledik
        public DateTime? TeslimTarihi { get; set; } 
    }
}