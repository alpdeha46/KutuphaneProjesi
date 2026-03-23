using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KutuphaneProjesi.Models
{
    public class Ogrenci
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string OkulNo { get; set; }
        
        // BURASI ÇOK KRİTİK: Sadece bunlar olmalı
        public int SinifId { get; set; } 
        public Sinif Sinif { get; set; } 

        public List<OduncIslem> OduncIslemleri { get; set; }
    }
}
