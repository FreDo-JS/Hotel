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
    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); 
        return RedirectToAction("Index", "Home"); 
    }
    [HttpPost]
    public async Task<IActionResult> GoogleLogin([FromBody] TokenDto tokenDto)
    {
        try
        {
            
            if (FirebaseApp.DefaultInstance == null)
            {
                return Json(new { success = false, message = "FirebaseApp nie została zainicjalizowana. Upewnij się, że Firebase Admin SDK jest poprawnie skonfigurowane." });
            }

            
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(tokenDto.Token);
            string uid = decodedToken.Uid;

            
            var email = decodedToken.Claims.ContainsKey("email") ? decodedToken.Claims["email"].ToString() : null;
            var name = decodedToken.Claims.ContainsKey("name") ? decodedToken.Claims["name"].ToString() : "Unknown User";

            
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == uid);

            if (existingUser == null)
            {
                
                var newUser = new User
                {
                    FirebaseUid = uid,
                    Email = email,
                    Name = name,
                    Role = "rezydent", 
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                
                HttpContext.Session.SetString("UserId", newUser.Id.ToString());
            }
            else
            {
                
                HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
            }

            
            if (!string.IsNullOrEmpty(name))
            {
                HttpContext.Session.SetString("UserName", name);
            }
            if (!string.IsNullOrEmpty(email))
            {
                HttpContext.Session.SetString("UserEmail", email);
            }
            if (!string.IsNullOrEmpty(existingUser.Role))
            {
                HttpContext.Session.SetString("UserRole", existingUser.Role);
            }


            
            var userData = decodedToken.Claims;
            HttpContext.Session.SetString("UserData", Newtonsoft.Json.JsonConvert.SerializeObject(userData));

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"Błąd weryfikacji tokenu: {ex}");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [RoleBasedAuthorize("administrator")]
    public IActionResult UserProfile()
    {
       
        var userDataJson = HttpContext.Session.GetString("UserData");
        if (userDataJson == null)
        {
            return RedirectToAction("", "Login");
        }

        var userData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(userDataJson);
        return View(userData);
    }

    public class TokenDto
    {
        public string? Token { get; set; }
    }

}
