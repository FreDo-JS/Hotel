using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Data;

namespace Hotel.Controllers
{
    public class UserPanelController : Controller
    {
        private readonly HotelDbContext _context;

        public UserPanelController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                Console.WriteLine("Nieprawidłowy UserId w sesji.");
                return RedirectToAction("", "login");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                Console.WriteLine($"Nie znaleziono użytkownika o UserId: {userId}");
                return RedirectToAction("", "login");
            }


            ViewBag.UserName = user.Name;
            ViewBag.FirstName = user.Name?.Split(' ')[0] ?? "Nieznane";
            ViewBag.LastName = user.Name?.Split(' ').Length > 1 ? user.Name.Split(' ')[1] : "Nieznane";
            ViewBag.Role = user.Role;
            ViewBag.Email = user.Email;


            
            var reservations = await _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Room)
                .Select(r => new
                {
                    ReservationId = r.Id,
                    RoomNumber = r.Room.RoomNumber,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    Status = r.Status
                })
                .ToListAsync();

            ViewBag.Reservations = reservations;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string newRole)
        {
            
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                TempData["Error"] = "Nie można zidentyfikować użytkownika. Zaloguj się ponownie.";
                return RedirectToAction("Index");
            }

            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                TempData["Error"] = "Nie znaleziono użytkownika.";
                return RedirectToAction("Index");
            }

            
            if (newRole == "rezydent" || newRole == "pracownik" || newRole == "administrator")
            {
                user.Role = newRole;
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("UserRole", newRole);

                TempData["Success"] = $"Rola została zmieniona na {newRole}.";
            }
            else
            {
                TempData["Error"] = "Wybrano nieprawidłową rolę.";
            }

            return RedirectToAction("Index");
        }

    }
} 

