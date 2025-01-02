using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Hotel.Data;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly HotelDbContext _context;

    public AccountController(HotelDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> GoogleLogin([FromBody] TokenDto tokenDto)
    {
        try
        {
            // Sprawdzenie, czy FirebaseApp została poprawnie zainicjalizowana
            if (FirebaseApp.DefaultInstance == null)
            {
                return Json(new { success = false, message = "FirebaseApp nie została zainicjalizowana. Upewnij się, że Firebase Admin SDK jest poprawnie skonfigurowane." });
            }

            // Weryfikacja tokenu identyfikacyjnego przy użyciu FirebaseAdmin
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(tokenDto.Token);
            string uid = decodedToken.Uid;

            // Pobierz dane użytkownika z tokenu
            var email = decodedToken.Claims.ContainsKey("email") ? decodedToken.Claims["email"].ToString() : null;
            var name = decodedToken.Claims.ContainsKey("name") ? decodedToken.Claims["name"].ToString() : "Unknown User";

            // Sprawdź, czy użytkownik już istnieje w bazie danych
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == uid);

            if (existingUser == null)
            {
                // Dodaj nowego użytkownika do bazy danych
                var newUser = new User
                {
                    FirebaseUid = uid,
                    Email = email,
                    Name = name,
                    Role = "rezydent", // Domyślna rola
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Zapisz ID nowego użytkownika w sesji
                HttpContext.Session.SetString("UserId", newUser.Id.ToString());
            }
            else
            {
                // Zapisz ID istniejącego użytkownika w sesji
                HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
            }

            // Przechowaj dane użytkownika w sesji
            if (!string.IsNullOrEmpty(name))
            {
                HttpContext.Session.SetString("UserName", name);
            }
            if (!string.IsNullOrEmpty(email))
            {
                HttpContext.Session.SetString("UserEmail", email);
            }

            // Przechowaj wszystkie dane użytkownika w sesji w formacie JSON
            var userData = decodedToken.Claims;
            HttpContext.Session.SetString("UserData", Newtonsoft.Json.JsonConvert.SerializeObject(userData));

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            // Zaloguj pełny komunikat błędu do konsoli
            Console.WriteLine($"Błąd weryfikacji tokenu: {ex}");
            return Json(new { success = false, message = ex.Message });
        }
    }


    public IActionResult UserProfile()
    {
        // Pobierz dane użytkownika z sesji
        var userDataJson = HttpContext.Session.GetString("UserData");
        if (userDataJson == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var userData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(userDataJson);
        return View(userData);
    }

    public class TokenDto
    {
        public string? Token { get; set; }
    }
}
