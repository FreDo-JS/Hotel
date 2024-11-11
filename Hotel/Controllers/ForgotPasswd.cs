using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class ForgotPasswd : Controller
    {
        public IActionResult forgotpasswd()
        {
            return View();
        }
    }
}
