using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    [HttpPost]
    public async Task<IActionResult> GoogleLogin([FromBody] TokenDto tokenDto)
    {
        try
        {
            // Weryfikacja tokenu identyfikacyjnego przy użyciu FirebaseAdmin
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(tokenDto.Token);
            string uid = decodedToken.Uid;

            // Zakładamy, że na podstawie UID tworzymy sesję użytkownika
            HttpContext.Session.SetString("UserId", uid);
            HttpContext.Session.SetString("UserName", decodedToken.Claims["name"].ToString());

            // Można przechowywać inne dane, np. e-mail
            if (decodedToken.Claims.ContainsKey("email"))
            {
                HttpContext.Session.SetString("UserEmail", decodedToken.Claims["email"].ToString());
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}

public class TokenDto
{
    public string Token { get; set; }
}
