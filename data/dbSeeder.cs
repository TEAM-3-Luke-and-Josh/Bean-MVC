using BeanScene.Models;
using Microsoft.EntityFrameworkCore;

namespace BeanScene.Data
{
    public static class DbSeeder
    {
        private static readonly Random _random = new Random();

        public static void Initialize(BeanSceneContext context)
        {
            // Apply any pending migrations
            context.Database.Migrate();

            // Clear all existing data
            ClearDatabase(context);

            // Seed the database
            SeedDatabase(context);
        }

        private static void ClearDatabase(BeanSceneContext context)
        {
            // Clear the data in the reverse order of dependencies
            context.Reservations.RemoveRange(context.Reservations);
            context.Sittings.RemoveRange(context.Sittings);
            context.Guests.RemoveRange(context.Guests);
            context.Tables.RemoveRange(context.Tables);
            context.Users.RemoveRange(context.Users);

            context.SaveChanges();
        }

        public static void SeedDatabase(BeanSceneContext context)
        {

            // 1. Seed Users (Manager, Staff, and Members)
            var users = new List<User>();

            // Add Manager
            users.Add(new User
            {
                Username = "manager",
                Password = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                Email = "manager@beanscene.com",
                UserType = "Manager"
            });

            // Add Staff
            users.Add(new User
            {
                Username = "staff",
                Password = BCrypt.Net.BCrypt.HashPassword("Staff@123"),
                Email = "staff@beanscene.com",
                UserType = "Staff"
            });

            // Add 10 Members
            for (int i = 1; i <= 10; i++)
            {
                users.Add(new User
                {
                    Username = $"member{i}",
                    Password = BCrypt.Net.BCrypt.HashPassword($"Member{i}@123"),
                    Email = $"member{i}@example.com",
                    UserType = "Member"
                });
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            // 2. Seed Tables
            var tables = new List<Table>
            {
                // Main Area Tables (M1-M10)
                new Table { TableID = "M1", Area = "Main", Capacity = 2 },
                new Table { TableID = "M2", Area = "Main", Capacity = 2 },
                new Table { TableID = "M3", Area = "Main", Capacity = 4 },
                new Table { TableID = "M4", Area = "Main", Capacity = 4 },
                new Table { TableID = "M5", Area = "Main", Capacity = 4 },
                new Table { TableID = "M6", Area = "Main", Capacity = 6 },
                new Table { TableID = "M7", Area = "Main", Capacity = 6 },
                new Table { TableID = "M8", Area = "Main", Capacity = 8 },
                new Table { TableID = "M9", Area = "Main", Capacity = 8 },
                new Table { TableID = "M10", Area = "Main", Capacity = 10 },

                // Outside Area Tables (O1-O10)
                new Table { TableID = "O1", Area = "Outside", Capacity = 2 },
                new Table { TableID = "O2", Area = "Outside", Capacity = 2 },
                new Table { TableID = "O3", Area = "Outside", Capacity = 4 },
                new Table { TableID = "O4", Area = "Outside", Capacity = 4 },
                new Table { TableID = "O5", Area = "Outside", Capacity = 4 },
                new Table { TableID = "O6", Area = "Outside", Capacity = 6 },
                new Table { TableID = "O7", Area = "Outside", Capacity = 6 },
                new Table { TableID = "O8", Area = "Outside", Capacity = 8 },
                new Table { TableID = "O9", Area = "Outside", Capacity = 8 },
                new Table { TableID = "O10", Area = "Outside", Capacity = 10 },

                // Balcony Area Tables (B1-B10)
                new Table { TableID = "B1", Area = "Balcony", Capacity = 2 },
                new Table { TableID = "B2", Area = "Balcony", Capacity = 2 },
                new Table { TableID = "B3", Area = "Balcony", Capacity = 4 },
                new Table { TableID = "B4", Area = "Balcony", Capacity = 4 },
                new Table { TableID = "B5", Area = "Balcony", Capacity = 4 },
                new Table { TableID = "B6", Area = "Balcony", Capacity = 6 },
                new Table { TableID = "B7", Area = "Balcony", Capacity = 6 },
                new Table { TableID = "B8", Area = "Balcony", Capacity = 8 },
                new Table { TableID = "B9", Area = "Balcony", Capacity = 8 },
                new Table { TableID = "B10", Area = "Balcony", Capacity = 10 }
            };

            context.Tables.AddRange(tables);
            context.SaveChanges();

            // 3. Seed Sittings for next 14 days
            var sittings = new List<Sitting>();
            var today = DateTime.Today;

            for (int day = 0; day < 14; day++)
            {
                var currentDate = today.AddDays(day);
                var isWeekend = currentDate.DayOfWeek == DayOfWeek.Saturday || 
                               currentDate.DayOfWeek == DayOfWeek.Sunday;

                // Breakfast Sitting (7 AM - 11 AM)
                sittings.Add(new Sitting
                {
                    SittingType = "Breakfast",
                    StartTime = currentDate.AddHours(7),
                    EndTime = currentDate.AddHours(11),
                    Capacity = isWeekend ? 80 : 60,
                    ClosedForReservations = false
                });

                // Lunch Sitting (11:30 AM - 3:30 PM)
                sittings.Add(new Sitting
                {
                    SittingType = "Lunch",
                    StartTime = currentDate.AddHours(11).AddMinutes(30),
                    EndTime = currentDate.AddHours(15).AddMinutes(30),
                    Capacity = isWeekend ? 100 : 80,
                    ClosedForReservations = false
                });

                // Dinner Sitting (5 PM - 10 PM)
                sittings.Add(new Sitting
                {
                    SittingType = "Dinner",
                    StartTime = currentDate.AddHours(17),
                    EndTime = currentDate.AddHours(22),
                    Capacity = isWeekend ? 100 : 80,
                    ClosedForReservations = false
                });
            }

            context.Sittings.AddRange(sittings);
            context.SaveChanges();

            // 4. Seed Guests (including member-linked guests)
            var guests = new List<Guest>();
            var memberUsers = users.Where(u => u.UserType == "Member").ToList();

            // Create guests for members
            foreach (var memberUser in memberUsers)
            {
                guests.Add(new Guest
                {
                    FirstName = $"Member{memberUser.Username.Replace("member", "")}",
                    LastName = "User",
                    Email = memberUser.Email,
                    PhoneNumber = $"04{_random.Next(10000000, 99999999)}"
                });
            }

            // Create additional random guests
            var firstNames = new[] { "James", "John", "Robert", "Mary", "Patricia", "Jennifer", "Michael", "William", "David", "Sarah" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };

            for (int i = 0; i < 40; i++)
            {
                guests.Add(new Guest
                {
                    FirstName = firstNames[_random.Next(firstNames.Length)],
                    LastName = lastNames[_random.Next(lastNames.Length)],
                    Email = $"guest{i}@example.com",
                    PhoneNumber = $"04{_random.Next(10000000, 99999999)}"
                });
            }

            context.Guests.AddRange(guests);
            context.SaveChanges();

            // 5. Seed Reservations
            var reservations = new List<Reservation>();
            var reservationStatuses = new[] { "Pending", "Confirmed", "Seated", "Completed", "Cancelled" };
            var specialRequests = new[] 
            { 
                "Window seat preferred",
                "Highchair needed",
                "Celebrating birthday",
                "Allergic to nuts",
                "Quiet table please",
                null,
                null,
                null
            };

            // Create 100 reservations spread across the next 14 days
            foreach (var sitting in sittings)
            {
                // Skip some sittings randomly to make distribution more realistic
                if (_random.Next(100) < 30) continue;

                var numReservations = _random.Next(2, 6); // 2-5 reservations per sitting
                for (int i = 0; i < numReservations; i++)
                {
                    var guest = guests[_random.Next(guests.Count)];
                    var startTime = sitting.StartTime.AddMinutes(_random.Next(0, (int)(sitting.EndTime - sitting.StartTime).TotalMinutes - 90));
                    var status = reservationStatuses[_random.Next(reservationStatuses.Length)];

                    // If the sitting is in the past, make sure status is either Completed or Cancelled
                    if (sitting.StartTime < DateTime.Now)
                    {
                        status = _random.Next(100) < 90 ? "Completed" : "Cancelled";
                    }

                    reservations.Add(new Reservation
                    {
                        GuestID = guest.GuestID,
                        SittingID = sitting.SittingID,
                        StartTime = startTime,
                        EndTime = startTime.AddMinutes(90),
                        NumberOfGuests = _random.Next(1, 9),
                        ReservationStatus = status,
                        Notes = specialRequests[_random.Next(specialRequests.Length)]
                    });
                }
            }

            context.Reservations.AddRange(reservations);
            context.SaveChanges();
        }
    }
}