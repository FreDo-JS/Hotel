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
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using MailKit.Net.Smtp;
using MimeKit;

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
            // Wygeneruj losowy kod QR
            var qrCode = GenerateRandomCode();

            // Utwórz nową rezerwację
            var reservation = new Reservation
            {
                UserId = reservationDto.UserId,
                RoomId = room.Id,
                LastName = reservationDto.LastName, // Dodanie nazwiska do zapisu
                CheckInDate = reservationDto.CheckInDate,
                CheckOutDate = reservationDto.CheckOutDate,
                Status = "potwierdzona",
                CreatedAt = DateTime.Now,
                QRCode = qrCode
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }



        [HttpPost]
        public async Task<IActionResult> CheckAvailability([FromBody] AvailabilityRequestDto request)
        {
            if (request.CheckInDate >= request.CheckOutDate)
            {
                return Json(new { success = false, message = "Data wyjazdu musi być późniejsza niż data przyjazdu." });
            }

            // Pobierz wszystkie pokoje na danym piętrze lub wszystkie piętra
            var roomsQuery = _context.Rooms.AsQueryable();
            if (!string.IsNullOrEmpty(request.Floor))
            {
                roomsQuery = roomsQuery.Where(r => r.Floor.ToString() == request.Floor);
            }

            var rooms = await roomsQuery.ToListAsync();

            // Pobierz zajęte pokoje w wybranym przedziale dat
            var reservations = await _context.Reservations
                .Where(r => r.CheckInDate < request.CheckOutDate && r.CheckOutDate > request.CheckInDate)
                .Include(r => r.User) // Dołącz dane użytkownika
                .ToListAsync();

            var result = rooms.Select(room => new
            {
                RoomNumber = room.RoomNumber,
                Status = reservations.Any(res => res.RoomId == room.Id) ? "zajęty" : "wolny",
                ReservationDetails = reservations
                    .Where(res => res.RoomId == room.Id)
                    .Select(res => new
                    {
                        UserName = res.User.Name,
                        UserLastName = res.LastName,
                        CheckInDate = res.CheckInDate.ToString("yyyy-MM-dd"),
                        CheckOutDate = res.CheckOutDate.ToString("yyyy-MM-dd")
                    })
            }).ToList();

            return Json(new { success = true, rooms = result });
        }
        private string GenerateRandomCode(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpGet]
        public async Task<IActionResult> GenerateNewQRCode(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
            {
                return Json(new { success = false, message = "Nie znaleziono rezerwacji." });
            }

            // Generowanie nowego losowego kodu
            reservation.QRCode = GenerateRandomCode();
            await _context.SaveChangesAsync();

            // Informacje do zapisania w kodzie QR
            var qrData = reservation.QRCode;

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCode(qrCodeData))
                {
                    using (var bitmap = qrCode.GetGraphic(20))
                    {
                        using (var stream = new MemoryStream())
                        {
                            bitmap.Save(stream, ImageFormat.Png);
                            var base64QRCode = Convert.ToBase64String(stream.ToArray());
                            return Json(new { success = true, qrCode = $"data:image/png;base64,{base64QRCode}" });
                        }
                    }
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetQRCode(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
            {
                return Json(new { success = false, message = "Nie znaleziono rezerwacji." });
            }

            if (string.IsNullOrEmpty(reservation.QRCode))
            {
                return Json(new { success = false, message = "Nie znaleziono kodu QR dla tej rezerwacji." });
            }

            // Generowanie QR kodu na podstawie istniejącego ciągu
            var qrData = reservation.QRCode;

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCode(qrCodeData))
                {
                    using (var bitmap = qrCode.GetGraphic(20))
                    {
                        using (var stream = new MemoryStream())
                        {
                            bitmap.Save(stream, ImageFormat.Png);
                            var base64QRCode = Convert.ToBase64String(stream.ToArray());
                            return Json(new { success = true, qrCode = $"data:image/png;base64,{base64QRCode}" });
                        }
                    }
                }
            }

        }
        [HttpPost]
        public async Task<IActionResult> SendQrCodeEmail([FromBody] ReservationEmailDto dto, [FromServices] EmailService emailService)
        {
            Console.WriteLine($"Próba wysłania kodu QR dla rezerwacji o ID: {dto.ReservationId}");

            if (dto.ReservationId <= 0)
            {
                Console.WriteLine("Niepoprawne ID rezerwacji.");
                return Json(new { success = false, message = "Niepoprawne ID rezerwacji." });
            }

            var reservation = await _context.Reservations
                .Include(r => r.User) // Pobranie użytkownika
                .FirstOrDefaultAsync(r => r.Id == dto.ReservationId);

            if (reservation == null)
            {
                Console.WriteLine($"Rezerwacja o ID {dto.ReservationId} nie została znaleziona.");
                return Json(new { success = false, message = "Nie znaleziono rezerwacji." });
            }

            if (string.IsNullOrEmpty(reservation.QRCode))
            {
                Console.WriteLine($"Rezerwacja o ID {dto.ReservationId} nie ma przypisanego kodu QR.");
                return Json(new { success = false, message = "Kod QR nie jest dostępny." });
            }

            if (reservation.User == null || string.IsNullOrEmpty(reservation.User.Email))
            {
                Console.WriteLine("Nie znaleziono adresu e-mail powiązanego użytkownika.");
                return Json(new { success = false, message = "Nie znaleziono adresu e-mail powiązanego użytkownika." });
            }

            try
            {
                Console.WriteLine($"Rozpoczynanie wysyłania kodu QR na adres: {reservation.User.Email}");
                await emailService.SendEmailWithQrCode(reservation.User.Email, reservation.QRCode);
                return Json(new { success = true, message = "Kod QR został wysłany na e-mail." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas wysyłania e-maila: {ex}");
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Wystąpił błąd podczas wysyłania e-maila." });
            }
        }





        public class EmailService
        {
            private readonly IConfiguration _configuration;

            public EmailService(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task SendEmailWithQrCode(string recipientEmail, string qrCodeData)
            {
                try
                {
                    Console.WriteLine($"QRCodeData: {qrCodeData}");

                    // Generowanie obrazu kodu QR na podstawie danych
                    byte[] qrCodeBytes;
                    using (var qrGenerator = new QRCodeGenerator())
                    {
                        var qrCode = new QRCode(qrGenerator.CreateQrCode(qrCodeData, QRCodeGenerator.ECCLevel.Q));
                        using (var bitmap = qrCode.GetGraphic(20))
                        {
                            using (var stream = new MemoryStream())
                            {
                                bitmap.Save(stream, ImageFormat.Png);
                                qrCodeBytes = stream.ToArray();
                            }
                        }
                    }

                    var emailSettings = _configuration.GetSection("EmailSettings");

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Hotel QR System", emailSettings["SenderEmail"]));
                    message.To.Add(new MailboxAddress("", recipientEmail));
                    message.Subject = "Twój kod QR";

                    var bodyBuilder = new BodyBuilder
                    {
                        HtmlBody = "<p>W załączniku znajdziesz swój kod QR. Użyj go do otwarcia drzwi swojego pokoju.</p>"
                    };

                    // Dołącz kod QR jako załącznik
                    bodyBuilder.Attachments.Add("QRCode.png", qrCodeBytes, new ContentType("image", "png"));
                    message.Body = bodyBuilder.ToMessageBody();

                    using (var client = new SmtpClient())
                    {
                        try
                        {
                            // Łączenie z serwerem SMTP
                            await client.ConnectAsync(
                                _configuration["EmailSettings:SmtpServer"],
                                int.Parse(_configuration["EmailSettings:SmtpPort"]),
                                MailKit.Security.SecureSocketOptions.SslOnConnect); // SSL dla portu 465

                            // Uwierzytelnianie
                            await client.AuthenticateAsync(
                                _configuration["EmailSettings:SenderEmail"],
                                _configuration["EmailSettings:SenderPassword"]);

                            // Wysyłanie e-maila
                            await client.SendAsync(message);
                            Console.WriteLine("E-mail został wysłany pomyślnie.");

                            // Rozłączanie
                            await client.DisconnectAsync(true);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Błąd podczas wysyłania e-maila: {ex.Message}");
                            throw;
                        }
                    }


                    Console.WriteLine("E-mail został wysłany.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas wysyłania e-maila: {ex.Message}");
                    throw;
                }
            }


        }
        [HttpGet]
        public async Task<IActionResult> ManagePendingReservations()
        {
            // Pobierz niepotwierdzone rezerwacje
            var pendingReservations = await _context.Reservations
                .Where(r => r.Status == "niepotwierdzona")
                .Include(r => r.User)
                .ToListAsync();

            // Pobierz listę dostępnych pokoi
            var availableRooms = await _context.Rooms
                .Where(r => !_context.Reservations.Any(res => res.RoomId == r.Id && res.Status == "potwierdzona"))
                .ToListAsync();

            ViewBag.PendingReservations = pendingReservations.Select(r => new
            {
                ReservationId = r.Id,
                LastName = r.LastName,
                CheckInDate = r.CheckInDate,
                CheckOutDate = r.CheckOutDate,
                Status = r.Status
            }).ToList();

            ViewBag.AvailableRooms = availableRooms.Select(r => new
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber
            }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmReservation(int reservationId, int roomId, [FromServices] EmailService emailService)
        {
            // Pobierz rezerwację na podstawie ID
            var reservation = await _context.Reservations.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null)
            {
                TempData["Error"] = "Nie znaleziono rezerwacji.";
                return RedirectToAction(nameof(ManagePendingReservations));
            }

            // Przypisz pokój do rezerwacji
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null)
            {
                TempData["Error"] = "Nie znaleziono pokoju.";
                return RedirectToAction(nameof(ManagePendingReservations));
            }

            reservation.RoomId = roomId;
            reservation.Status = "potwierdzona";

            // Generowanie losowego kodu QR
            reservation.QRCode = GenerateRandomCode();

            await _context.SaveChangesAsync();

            // Wyślij e-mail z kodem QR
            if (reservation.User != null && !string.IsNullOrEmpty(reservation.User.Email))
            {
                try
                {
                    Console.WriteLine($"Wysyłanie kodu QR na adres: {reservation.User.Email}");
                    await emailService.SendEmailWithQrCode(reservation.User.Email, reservation.QRCode);
                    TempData["Success"] = $"Rezerwacja {reservationId} potwierdzona i kod QR został wysłany na adres {reservation.User.Email}.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas wysyłania e-maila: {ex.Message}");
                    TempData["Error"] = $"Rezerwacja {reservationId} została potwierdzona, ale nie udało się wysłać kodu QR.";
                }
            }
            else
            {
                TempData["Error"] = "Nie znaleziono adresu e-mail użytkownika.";
            }

            return RedirectToAction(nameof(ManagePendingReservations));
        }

        [HttpGet]
        public async Task<IActionResult> DatabaseOverview()
        {
            // Pobierz dane z tabel
            var users = await _context.Users.ToListAsync();
            var rooms = await _context.Rooms.ToListAsync();
            var reservations = await _context.Reservations.Include(r => r.User).Include(r => r.Room).ToListAsync();

            // Przekaż dane do widoku
            ViewBag.Users = users;
            ViewBag.Rooms = rooms;
            ViewBag.Reservations = reservations;

            return View();
        }



        public class ReservationEmailDto
        {
            public int ReservationId { get; set; }
        }


        public class AvailabilityRequestDto
        {
            public DateTime CheckInDate { get; set; }
            public DateTime CheckOutDate { get; set; }
            public string? Floor { get; set; }
        }




    }

    public class ReservationDto
    {
        public int UserId { get; set; }
        public string? LastName { get; set; } // Dodane nazwisko
        public int RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }

}
