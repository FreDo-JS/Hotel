using Microsoft.AspNetCore.Mvc;

namespace Hotel.Views.UserPanel
{
    public class UserPanelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
