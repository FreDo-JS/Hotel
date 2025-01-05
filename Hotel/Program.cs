using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hotel.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using static Hotel.Controllers.ServiceController;

var builder = WebApplication.CreateBuilder(args);

// Sesja 10 min
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(10);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// Inicjalizacja Firebase ale w zasadzie korzystamy tylko z autha
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

// Konfiguracja MySQL z appsettings
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection")
	?? throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");


EnsureDatabaseExists(connectionString);


builder.Services.AddDbContext<HotelDbContext>(options =>
	options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<HotelDbContext>();

// Rejestracja us³ug
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<EmailService>();

var app = builder.Build();

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
	name: "qr",
	pattern: "Home/ScanQrCode",
	defaults: new { controller = "Home", action = "ScanQrCode" });

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// automat do tworzenia bazy do testów
void EnsureDatabaseExists(string connectionString)
{
	try
	{
		var builder = new MySqlConnectionStringBuilder(connectionString);
		var databaseName = builder.Database;

		builder.Database = null; 
		using var connection = new MySqlConnection(builder.ConnectionString);
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{databaseName}`;";
		command.ExecuteNonQuery();

		Console.WriteLine($"Baza danych '{databaseName}' zosta³a utworzona lub ju¿ istnieje.");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"B³¹d podczas sprawdzania lub tworzenia bazy danych: {ex.Message}");
		throw;
	}
}
