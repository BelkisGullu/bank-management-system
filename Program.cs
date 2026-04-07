using BankManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Database Connection - SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=BankManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Session Configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages routing
app.MapRazorPages();

// Seed initial data - Veritabanının var olduğundan emin ol
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Veritabanının var olduğunu kontrol et, yoksa oluştur
        if (context.Database.CanConnect())
        {
            // Migration'ları uygula (eğer yoksa)
            context.Database.Migrate();
            
            // Seed data ekle
            DbInitializer.Initialize(context);
        }
        else
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Veritabanına bağlanılamıyor. Lütfen 'dotnet ef database update' komutunu çalıştırın.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı başlatma hatası: {Message}", ex.Message);
        logger.LogWarning("Proje çalışmaya devam ediyor, ancak veritabanı hazır olmayabilir. Lütfen migration'ı çalıştırın: dotnet ef database update");
    }
}

app.Run();
