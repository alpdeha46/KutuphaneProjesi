using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Models;

namespace KutuphaneProjesi.Data
{
    public class KutuphaneContext : DbContext
    {
        public KutuphaneContext(DbContextOptions<KutuphaneContext> options) : base(options) { }

        public DbSet<Kitap> Kitaplar { get; set; }
        public DbSet<Ogrenci> Ogrenciler { get; set; }
        public DbSet<Sinif> Siniflar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<OduncIslem> OduncIslemleri { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sınıf seviyesi ve şube ikilisini benzersiz yap (Mükerrer kaydı veritabanında engeller)
            modelBuilder.Entity<Sinif>()
                .HasIndex(s => new { s.Seviye, s.Sube })
                .IsUnique();

            // İlişkileri yapılandır (Öğrenci silinince ödünç işlemleri de gitsin ama sınıf kalmaya devam etsin)
            modelBuilder.Entity<Ogrenci>()
                .HasOne(o => o.Sinif)
                .WithMany(s => s.Ogrenciler)
                .HasForeignKey(o => o.SinifId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}