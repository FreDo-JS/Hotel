using Firebase.Auth;
using FirebaseAdmin;
using Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hotel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        FirebaseAuthProvider _firebaseAuth;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _firebaseAuth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDDOcI0a2y3vNwrwvzzcqHkN0p_JHUvKbI"));
        }

        public IActionResult Index()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                Console.WriteLine("FirebaseApp nie zosta³o zainicjalizowane.");
                return Content("FirebaseApp nie zosta³o zainicjalizowane.");
            }
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                // U¿ytkownik nie jest zalogowany, przekierowanie do logowania
                return RedirectToAction("", "Login");
            }

            // U¿ytkownik jest zalogowany, przekazanie danych do widoku
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult TOMABYCGLOWNA()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
