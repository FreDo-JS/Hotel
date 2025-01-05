using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class GłównaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
