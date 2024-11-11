using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class Register : Controller
    {
        public IActionResult register()
        {
            return View();
        }
    }
}
