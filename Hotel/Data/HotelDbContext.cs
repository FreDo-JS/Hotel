using Microsoft.EntityFrameworkCore;

namespace Hotel.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

        // Tabela użytkowników
        public DbSet<User> Users { get; set; }

        // Tabela pokoi
        public DbSet<Room> Rooms { get; set; }

        // Tabela statystyk pokoi
        public DbSet<RoomStatistic> RoomStatistics { get; set; }

        public DbSet<Reservation> Reservations { get; set; } // Dodanie tabeli rezerwacji
       
    }

    // Model dla tabeli Users
    public class User
    {
        public int Id { get; set; }
        public string? FirebaseUid { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    // Model dla tabeli Rooms
    public class Room
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public int Floor { get; set; }
        public string? Status { get; set; }
        public int? ResidentId { get; set; }
        public User? Resident { get; set; }
    }

    // Model dla tabeli RoomStatistics
    public class RoomStatistic
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public int TotalReservations { get; set; }
        public DateTime? LastCleaned { get; set; }
        public int CurrentOccupancy { get; set; }
    }
    public class Reservation
    {
        public int Id { get; set; }         
        public int UserId { get; set; }
        public User? User { get; set; } 
        public int RoomId { get; set; }
        public Room? Room { get; set; } 
        public DateTime CheckInDate { get; set; } 
        public DateTime CheckOutDate { get; set; }
        public string? LastName { get; set; } // Nazwisko użytkownika
        public string? Status { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }

}
