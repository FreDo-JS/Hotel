using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class ForgotPassword : Controller
    {
        public IActionResult index()
        {
            return View();
        }
    }
}
