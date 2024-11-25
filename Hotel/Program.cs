using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hotel.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//ad Sessions

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(20); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

});
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
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
app.MapRazorPages();

app.Run();
