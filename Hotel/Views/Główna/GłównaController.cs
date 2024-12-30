using Microsoft.AspNetCore.Mvc;

namespace Hotel.Views.Główna
{
    public class GłównaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
