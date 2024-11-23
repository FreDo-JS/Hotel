using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult index()
        {
            return View();
        }
    }
}
