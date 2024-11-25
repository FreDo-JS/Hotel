using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
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

            // Zakładamy, że na podstawie UID tworzymy sesję użytkownika
            HttpContext.Session.SetString("UserId", uid);

            // Przechowaj dane użytkownika w sesji
            if (decodedToken.Claims.ContainsKey("name"))
            {
                HttpContext.Session.SetString("UserName", decodedToken.Claims["name"].ToString());
            }
            if (decodedToken.Claims.ContainsKey("email"))
            {
                HttpContext.Session.SetString("UserEmail", decodedToken.Claims["email"].ToString());
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
        public string Token { get; set; }
    }
}