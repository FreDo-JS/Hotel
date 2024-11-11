using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class Login : Controller
    {
        public IActionResult log()
        {
            return View();
        }
    }
}
