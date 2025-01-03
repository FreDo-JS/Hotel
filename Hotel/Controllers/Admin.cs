using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class Admin : Controller
    {
        public IActionResult index()
        {
            return View();
        }
        public IActionResult qrCode()
        {
            return View();
        }
    }
   
}
