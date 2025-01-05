using Microsoft.EntityFrameworkCore;

namespace Hotel.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

        
        public DbSet<User> Users { get; set; }

       
        public DbSet<Room> Rooms { get; set; }

        
        public DbSet<RoomStatistic> RoomStatistics { get; set; }

        public DbSet<Reservation> Reservations { get; set; } // Dodanie tabeli rezerwacji
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Dodanie użytkowników
			modelBuilder.Entity<User>().HasData(
				new User { Id = 1, Name = "Jan Kowalski", Email = "j4n.k0wal4kisac1k@gmail.com", Role = "rezydent", FirebaseUid = "kasd782m2akd8922s" },
				new User { Id = 2, Name = "Anna Lewandowska", Email = "4nn4l3wa9s1lsp9smki@gmail.com", Role = "rezydent", FirebaseUid = "d7j89s12mdm12" }
			);

			// Dodanie pokoi
			modelBuilder.Entity<Room>().HasData(
				new Room { Id = 1, RoomNumber = 101, Floor = 1, Status = "wolny" },
				new Room { Id = 2, RoomNumber = 102, Floor = 1, Status = "wolny" },
				new Room { Id = 3, RoomNumber = 103, Floor = 1, Status = "wolny" },
				new Room { Id = 4, RoomNumber = 104, Floor = 1, Status = "wolny" },
				new Room { Id = 5, RoomNumber = 105, Floor = 1, Status = "wolny" },
				new Room { Id = 6, RoomNumber = 201, Floor = 2, Status = "wolny" },
				new Room { Id = 7, RoomNumber = 202, Floor = 2, Status = "wolny" },
				new Room { Id = 8, RoomNumber = 203, Floor = 2, Status = "wolny" },
				new Room { Id = 9, RoomNumber = 204, Floor = 2, Status = "wolny" },
				new Room { Id = 10, RoomNumber = 301, Floor = 3, Status = "wolny" },
				new Room { Id = 11, RoomNumber = 302, Floor = 3, Status = "wolny" },
				new Room { Id = 12, RoomNumber = 303, Floor = 3, Status = "wolny" },
				new Room { Id = 13, RoomNumber = 304, Floor = 3, Status = "wolny" }
			);

			// Dodanie rezerwacji
			modelBuilder.Entity<Reservation>().HasData(
				new Reservation
				{
					Id = 1,
					UserId = 1,
					RoomId = 3,
					CheckInDate = DateTime.Now.AddDays(-3),
					CheckOutDate = DateTime.Now.AddDays(4),
					Status = "potwierdzona",
					LastName = "Kowalski",
					QRCode = "821928da892d"
				}
			);
		}
	}

    
    public class User
    {
        public int Id { get; set; }
        public string? FirebaseUid { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    
    public class Room
    {
        public int Id { get; set; }
        public int? RoomNumber { get; set; }
        public int Floor { get; set; }
        public string? Status { get; set; }
        public int? ResidentId { get; set; }
        public User? Resident { get; set; }
    }

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
        public int? RoomId { get; set; }
        public Room? Room { get; set; } 
        public DateTime CheckInDate { get; set; } 
        public DateTime CheckOutDate { get; set; }
        public string? LastName { get; set; } // Nazwisko użytkownika
        public string? QRCode { get; set; }
        public string? Status { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now; 

    }
	

}
