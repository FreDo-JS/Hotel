using Firebase.Auth;
using FirebaseAdmin.Auth;
using Hotel.Data;
using Microsoft.AspNetCore.Mvc;
using FirebaseAdminAuth = FirebaseAdmin.Auth.FirebaseAuth;
using Microsoft.EntityFrameworkCore;
using HotelUser = Hotel.Data.User;


public class AuthController : Controller
{
    private readonly HotelDbContext _context;

    public AuthController(HotelDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> VerifyToken([FromBody] string idToken)
    {
        try
        {
            
            FirebaseToken decodedToken = await FirebaseAdminAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

            string uid = decodedToken.Uid;
            string email = decodedToken.Claims["email"]?.ToString() ?? "";
            string name = decodedToken.Claims["name"]?.ToString() ?? "";

            
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == uid);
            if (existingUser == null)
            {
                var newUser = new Hotel.Data.User
                {
                    FirebaseUid = uid,
                    Email = email,
                    Name = name,
                    Role = "rezydent"
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
            }


            return Json(new { success = true, message = "Użytkownik zweryfikowany.", existingUser });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Błąd weryfikacji: {ex.Message}" });
        }
    }
}
