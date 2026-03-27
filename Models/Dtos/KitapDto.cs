namespace KutuphaneProjesi.Models
{
    public class KitapDto
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string? Yazar { get; set; }
        public int StokAdedi { get; set; }
        public int SayfaSayisi { get; set; }
        public int KategoriId { get; set; }
    }
}