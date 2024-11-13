using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class Service : Controller
    {
        public IActionResult index()
        {
            return View();
        }
    }
}
