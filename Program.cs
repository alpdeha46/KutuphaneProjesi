using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantısı (SQLite)
builder.Services.AddDbContext<KutuphaneContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Controller ve API Desteği
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer(); // API keşfi için

// 3. KRİTİK: CORS İZNİ (Flutter'ın API'ye erişebilmesi için şart!)
builder.Services.AddCors(options =>
{
    options.AddPolicy("FlutterPolicy", policy =>
    {
        policy.AllowAnyOrigin()  // Şimdilik her yerden erişime izin ver
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 4. Veritabanını Otomatik Doldur (Seed Data)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<KutuphaneContext>();
        DbSeeder.Seed(context); // Senin yazdığın o efsane seeder
    }
    catch (Exception ex)
    {
        Console.WriteLine("Seed Hatası: " + ex.Message);
    }
}

// 5. HTTP Yapılandırması
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Flutter testi sırasında sertifika hatası almamak için 
// Geliştirme aşamasında HTTPS yönlendirmesini kapatabilirsin (opsiyonel)
// app.UseHttpsRedirection(); 

app.UseStaticFiles();
app.UseRouting();

// 6. CORS Politikasını Aktif Et
app.UseCors("FlutterPolicy");

app.UseAuthorization();

// 7. Rotalar (Hem MVC hem API için)
app.MapControllers(); // API'ler için: api/KutuphaneApi/...

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();