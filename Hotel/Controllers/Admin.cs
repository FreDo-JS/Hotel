using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class Admin : Controller
    {
        public IActionResult index()
        {
            return View();
        }
    }
}
