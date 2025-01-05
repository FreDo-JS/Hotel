using Firebase.Auth;
using FirebaseAdmin;
using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost]
        public async Task<IActionResult> CheckQrCode([FromBody] QrCodeDto qrCodeDto)
        {
            if (string.IsNullOrEmpty(qrCodeDto.QrCode))
            {
                return Json(new { success = false, message = "Kod QR jest wymagany." });
            }

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.QRCode == qrCodeDto.QrCode);

            if (reservation == null)
            {
                return Json(new { success = false, message = "Nie znaleziono rezerwacji dla tego kodu QR." });
            }

            return Json(new { success = true, roomNumber = reservation.Room?.RoomNumber });
        }


        public class QrCodeDto
        {
            public string QrCode { get; set; }
        }


        [HttpGet]
        public IActionResult ScanQrCode()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("UserName") ?? "Goœæ";
            string userSurname = HttpContext.Session.GetString("UserEmail") ?? "";
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            if (FirebaseApp.DefaultInstance == null)
            {
                Console.WriteLine("FirebaseApp nie zosta³o zainicjalizowane.");
                return Content("FirebaseApp nie zosta³o zainicjalizowane.");
            }
            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(userId);
            /*if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                // U¿ytkownik nie jest zalogowany, przekierowanie do logowania
                return RedirectToAction("", "Login");
            }*/

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
            
            string userName = HttpContext.Session.GetString("UserName") ?? "Goœæ";
            string userSurname = HttpContext.Session.GetString("UserEmail") ?? "";

            
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
                return Json(new { success = false, message = "Data wyjazdu musi byæ póŸniejsza ni¿ data przyjazdu." });
            }

            
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "Musisz byæ zalogowany, aby dokonaæ rezerwacji." });

            }

            
            var newReservation = new Reservation
            {
                UserId = userId,
                LastName = reservationDto.LastName,
                CheckInDate = reservationDto.CheckInDate,
                CheckOutDate = reservationDto.CheckOutDate,
                Status = "niepotwierdzona",
                CreatedAt = DateTime.Now,
                QRCode = null, 
                RoomId = null  
            };

            _context.Reservations.Add(newReservation);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Rezerwacja zosta³a zapisana do zatwierdzenia." });
        }


    }

}
