using Firebase.Auth;
using FirebaseAdmin;
using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hotel.Controllers
{
    public class HomeController : Controller
    {
        

        private readonly ILogger<HomeController> _logger;
        private readonly HotelDbContext _context;
        private readonly FirebaseAuthProvider _firebaseAuth;

        public HomeController(ILogger<HomeController> logger, HotelDbContext context)
        {
            _logger = logger;
            _context = context;
            _firebaseAuth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDDOcI0a2y3vNwrwvzzcqHkN0p_JHUvKbI"));
        }


        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("UserName") ?? "Go��";
            string userSurname = HttpContext.Session.GetString("UserEmail") ?? "";
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            if (FirebaseApp.DefaultInstance == null)
            {
                Console.WriteLine("FirebaseApp nie zosta�o zainicjalizowane.");
                return Content("FirebaseApp nie zosta�o zainicjalizowane.");
            }
            /*if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                // U�ytkownik nie jest zalogowany, przekierowanie do logowania
                return RedirectToAction("", "Login");
            }*/

            // U�ytkownik jest zalogowany, przekazanie danych do widoku
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult TOMABYCGLOWNA()
        {
            // Pobranie danych u�ytkownika z sesji
            string userName = HttpContext.Session.GetString("UserName") ?? "Go��";
            string userSurname = HttpContext.Session.GetString("UserEmail") ?? "";

            // Przekazanie danych do widoku
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;

            return View();
        }


        private string GenerateRandomCode(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public async Task<IActionResult> CreatePublicReservation([FromBody] ReservationDto reservationDto)
        {
            if (reservationDto.CheckInDate >= reservationDto.CheckOutDate)
            {
                return Json(new { success = false, message = "Data wyjazdu musi by� p�niejsza ni� data przyjazdu." });
            }

            // Pobierz ID u�ytkownika z sesji
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "Musisz by� zalogowany, aby dokona� rezerwacji." });
            }

            // Utw�rz now� rezerwacj� bez przypisania pokoju
            var newReservation = new Reservation
            {
                UserId = userId,
                LastName = reservationDto.LastName,
                CheckInDate = reservationDto.CheckInDate,
                CheckOutDate = reservationDto.CheckOutDate,
                Status = "niepotwierdzona",
                CreatedAt = DateTime.Now,
                QRCode = null, // Kod QR nie jest generowany na tym etapie
                RoomId = null  // Nie przypisujemy pokoju
            };

            _context.Reservations.Add(newReservation);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Rezerwacja zosta�a zapisana do zatwierdzenia." });
        }


    }

}
