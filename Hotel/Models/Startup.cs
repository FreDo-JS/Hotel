namespace Hotel.Models
{
    using FirebaseAdmin;
    using Google.Apis.Auth.OAuth2;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSession(); // Dodaj sesję

            // Inicjalizacja Firebase Admin SDK
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("ścieżka/do/twojego/klucza.json")
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

            app.UseSession(); // Użyj sesji

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

}
