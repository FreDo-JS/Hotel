using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using QRCoder;
using System;
using System.IO;
using System.Threading.Tasks;
using Hotel.Data;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Controllers
{
    public class ServiceController : Controller
    {
        private readonly HotelDbContext _context;
        public ServiceController(HotelDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Pobierz listę pokoi z bazy danych
            var rooms = await _context.Rooms.Select(r => new { r.Id, r.RoomNumber }).ToListAsync();
            ViewBag.Rooms = rooms; // Przekazanie listy pokoi do widoku

            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> DodajPokoj()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return View(rooms);
        }
        


        // Dodanie nowego pokoju
        [HttpPost]
        public async Task<IActionResult> DodajPokoj(Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DodajPokoj)); // Powrót do widoku po dodaniu
            }

            var rooms = await _context.Rooms.ToListAsync();
            return View(rooms);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationDto reservationDto)
        {
            if (reservationDto.CheckInDate >= reservationDto.CheckOutDate)
            {
                return Json(new { success = false, message = "Data wyjazdu musi być późniejsza niż data przyjazdu." });
            }

            // Pobierz ID pokoju na podstawie numeru pokoju
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == reservationDto.RoomNumber);
            if (room == null)
            {
                return Json(new { success = false, message = "Nie znaleziono pokoju o podanym numerze." });
            }

            // Sprawdź dostępność pokoju
            var isAvailable = !await _context.Reservations.AnyAsync(r =>
                r.RoomId == room.Id &&
                ((reservationDto.CheckInDate >= r.CheckInDate && reservationDto.CheckInDate < r.CheckOutDate) ||
                 (reservationDto.CheckOutDate > r.CheckInDate && reservationDto.CheckOutDate <= r.CheckOutDate) ||
                 (reservationDto.CheckInDate <= r.CheckInDate && reservationDto.CheckOutDate >= r.CheckOutDate)));

            if (!isAvailable)
            {
                return Json(new { success = false, message = "Pokój jest zajęty w wybranym terminie." });
            }

            // Utwórz nową rezerwację
            var reservation = new Reservation
            {
                UserId = reservationDto.UserId,
                RoomId = room.Id,
                CheckInDate = reservationDto.CheckInDate,
                CheckOutDate = reservationDto.CheckOutDate,
                Status = "potwierdzona",
                CreatedAt = DateTime.Now
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }





    }

    public class ReservationDto
    {
        public int UserId { get; set; }
        public int RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
