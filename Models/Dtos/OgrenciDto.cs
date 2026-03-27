namespace KutuphaneProjesi.Models
{
    public class OgrenciDto
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string OkulNo { get; set; } = string.Empty;
        public int SinifId { get; set; }
    }
}