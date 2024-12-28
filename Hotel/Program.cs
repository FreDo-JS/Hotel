using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hotel.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dodanie sesji
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Inicjalizacja Firebase
var keyFilePath = Path.Combine(builder.Environment.ContentRootPath, "Config", "hotelssigma-firebase-adminsdk-5k6yn-f40a7fbbd6.json");
if (File.Exists(keyFilePath))
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(keyFilePath)
    });
}
else
{
    throw new FileNotFoundException($"Plik klucza Firebase nie zosta³ znaleziony: {keyFilePath}");
}

// Konfiguracja MySQL
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Rejestracja Identity z u¿yciem bazy danych
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();


// Rejestracja kontrolerów
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfiguracja middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
