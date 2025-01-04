using FirebaseAdmin;
using Hotel.Data;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Hosting;

namespace Hotel.Models
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSession(); // Dodaj sesję

            try
            {
                
                if (FirebaseApp.DefaultInstance == null)
                {
                    var keyFilePath = Path.Combine(_env.ContentRootPath, "Config", "hotelssigma-firebase-adminsdk-5k6yn-f40a7fbbd6.json");
                    if (!System.IO.File.Exists(keyFilePath))
                    {
                        throw new FileNotFoundException($"Plik klucza Firebase nie został znaleziony w lokalizacji: {keyFilePath}");
                    }

                    Console.WriteLine($"Próba inicjalizacji Firebase z pliku: {keyFilePath}");

                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(keyFilePath)
                    });

                    Console.WriteLine("FirebaseApp została pomyślnie zainicjalizowana.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas inicjalizacji Firebase Admin SDK: {ex.Message}");
            }

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Czas trwania sesji
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession(); 

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
