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

            // Clear previous data
            ClearDatabase(context);

            // Seed reservations
            SeedReservations(context);

            // Seed menu items
            SeedMenuData(context);

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

        public static void SeedReservations(BeanSceneContext context)
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
                
                // Get all tables for random assignment
                var availableTables = tables.ToList();
                
                for (int i = 0; i < numReservations; i++)
                {
                    var guest = guests[_random.Next(guests.Count)];
                    var startTime = sitting.StartTime.AddMinutes(_random.Next(0, (int)(sitting.EndTime - sitting.StartTime).TotalMinutes - 90));
                    var status = reservationStatuses[_random.Next(reservationStatuses.Length)];
                    var numberOfGuests = _random.Next(1, 9);

                    // If the sitting is in the past, make sure status is either Completed or Cancelled
                    if (sitting.StartTime < DateTime.Now)
                    {
                        status = _random.Next(100) < 90 ? "Completed" : "Cancelled";
                    }

                    // Assign appropriate tables based on party size
                    var assignedTables = new List<Table>();
                    int remainingCapacity = numberOfGuests;
                    while (remainingCapacity > 0 && availableTables.Any())
                    {
                        // Find suitable tables (with capacity close to remaining guests)
                        var suitableTables = availableTables
                            .Where(t => t.Capacity <= remainingCapacity + 2) // Allow some overflow
                            .OrderBy(t => Math.Abs(t.Capacity - remainingCapacity))
                            .ToList();

                        if (!suitableTables.Any())
                        {
                            // If no suitable tables, just take the smallest available
                            suitableTables = availableTables
                                .OrderBy(t => t.Capacity)
                                .ToList();
                        }

                        var selectedTable = suitableTables.First();
                        assignedTables.Add(selectedTable);
                        remainingCapacity -= selectedTable.Capacity;
                    }

                    var reservation = new Reservation
                    {
                        GuestID = guest.GuestID,
                        SittingID = sitting.SittingID,
                        StartTime = startTime,
                        EndTime = startTime.AddMinutes(90),
                        NumberOfGuests = numberOfGuests,
                        ReservationStatus = status,
                        Notes = specialRequests[_random.Next(specialRequests.Length)],
                        Tables = assignedTables  // Assign the tables to the reservation
                    };

                    reservations.Add(reservation);
                }
            }

            context.Reservations.AddRange(reservations);
            context.SaveChanges();
        }

        public static void SeedMenuData(BeanSceneContext context)
        {
            // Skip if menu data already exists
            if (context.MenuCategories.Any()) return;

            try
            {
                // 1. Create Categories
                var categories = new List<MenuCategory>
                {
                    new MenuCategory { Name = "Breakfast", Description = "Start your day right" },
                    new MenuCategory { Name = "Lunch Mains", Description = "Midday favorites" },
                    new MenuCategory { Name = "Dinner Mains", Description = "Evening specialties" },
                    new MenuCategory { Name = "Sides", Description = "Perfect accompaniments" },
                    new MenuCategory { Name = "Beverages", Description = "Drinks selection" },
                    new MenuCategory { Name = "Desserts", Description = "Sweet endings" }
                };

                context.MenuCategories.AddRange(categories);
                context.SaveChanges();

                // 2. Create Menu Items (without options first)
                var breakfastItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        CategoryID = categories[0].CategoryID,
                        Name = "Eggs Benedict",
                        Description = "Poached eggs, ham, hollandaise on English muffin",
                        Price = 22.00M,
                        PrepTime = 15
                    },
                    new MenuItem
                    {
                        CategoryID = categories[0].CategoryID,
                        Name = "Avocado Toast",
                        Description = "Smashed avocado, feta, cherry tomatoes on sourdough",
                        Price = 18.00M,
                        PrepTime = 10
                    }
                };

                var lunchItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        CategoryID = categories[1].CategoryID,
                        Name = "Beef Burger",
                        Description = "House-made beef patty, lettuce, tomato, cheese, special sauce",
                        Price = 24.00M,
                        PrepTime = 20
                    },
                    new MenuItem
                    {
                        CategoryID = categories[1].CategoryID,
                        Name = "Caesar Salad",
                        Description = "Cos lettuce, croutons, parmesan, bacon, Caesar dressing",
                        Price = 19.00M,
                        PrepTime = 12
                    }
                };

                var dinnerItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        CategoryID = categories[2].CategoryID,
                        Name = "Grilled Ribeye Steak",
                        Description = "300g ribeye, herb butter, red wine jus",
                        Price = 42.00M,
                        PrepTime = 25
                    },
                    new MenuItem
                    {
                        CategoryID = categories[2].CategoryID,
                        Name = "Pan-Seared Salmon",
                        Description = "Atlantic salmon, crushed potatoes, broccolini, lemon butter",
                        Price = 36.00M,
                        PrepTime = 20
                    }
                };

                var sides = new List<MenuItem>
                {
                    new MenuItem
                    {
                        CategoryID = categories[3].CategoryID,
                        Name = "Sweet Potato Fries",
                        Description = "Served with aioli",
                        Price = 9.00M,
                        PrepTime = 10
                    },
                    new MenuItem
                    {
                        CategoryID = categories[3].CategoryID,
                        Name = "Garden Salad",
                        Description = "Mixed leaves, cherry tomatoes, cucumber, house dressing",
                        Price = 8.00M,
                        PrepTime = 5
                    }
                };

                var beverages = new List<MenuItem>
                {
                    new MenuItem
                    {
                        CategoryID = categories[4].CategoryID,
                        Name = "Fresh Orange Juice",
                        Description = "Freshly squeezed oranges",
                        Price = 6.00M,
                        PrepTime = 5
                    },
                    new MenuItem
                    {
                        CategoryID = categories[4].CategoryID,
                        Name = "Flat White",
                        Description = "Double shot coffee with steamed milk",
                        Price = 4.50M,
                        PrepTime = 5
                    }
                };

                var desserts = new List<MenuItem>
                {
                    new MenuItem
                    {
                        CategoryID = categories[5].CategoryID,
                        Name = "Sticky Date Pudding",
                        Description = "Butterscotch sauce, vanilla ice cream",
                        Price = 14.00M,
                        PrepTime = 15
                    },
                    new MenuItem
                    {
                        CategoryID = categories[5].CategoryID,
                        Name = "Chocolate Fondant",
                        Description = "Warm chocolate cake, liquid center, vanilla ice cream",
                        Price = 16.00M,
                        PrepTime = 20
                    }
                };

                // Add all menu items
                var allItems = new List<MenuItem>();
                allItems.AddRange(breakfastItems);
                allItems.AddRange(lunchItems);
                allItems.AddRange(dinnerItems);
                allItems.AddRange(sides);
                allItems.AddRange(beverages);
                allItems.AddRange(desserts);

                context.MenuItems.AddRange(allItems);
                context.SaveChanges();

                // 3. Add options to items
                var itemOptions = new List<ItemOption>();

                // Eggs Benedict options
                itemOptions.AddRange(new[]
                {
                    new ItemOption { ItemID = breakfastItems[0].ItemID, Name = "Extra Egg", PriceModifier = 3.00M },
                    new ItemOption { ItemID = breakfastItems[0].ItemID, Name = "Smoked Salmon Instead of Ham", PriceModifier = 4.00M }
                });

                // Avocado Toast options
                itemOptions.AddRange(new[]
                {
                    new ItemOption { ItemID = breakfastItems[1].ItemID, Name = "Add Poached Egg", PriceModifier = 3.00M },
                    new ItemOption { ItemID = breakfastItems[1].ItemID, Name = "Add Bacon", PriceModifier = 4.00M }
                });

                // Burger options
                itemOptions.AddRange(new[]
                {
                    new ItemOption { ItemID = lunchItems[0].ItemID, Name = "Extra Patty", PriceModifier = 6.00M },
                    new ItemOption { ItemID = lunchItems[0].ItemID, Name = "Add Bacon", PriceModifier = 4.00M },
                    new ItemOption { ItemID = lunchItems[0].ItemID, Name = "Gluten-Free Bun", PriceModifier = 2.00M }
                });

                // Flat White options
                itemOptions.AddRange(new[]
                {
                    new ItemOption { ItemID = beverages[1].ItemID, Name = "Extra Shot", PriceModifier = 0.50M },
                    new ItemOption { ItemID = beverages[1].ItemID, Name = "Soy Milk", PriceModifier = 0.50M },
                    new ItemOption { ItemID = beverages[1].ItemID, Name = "Almond Milk", PriceModifier = 0.50M }
                });

                context.ItemOptions.AddRange(itemOptions);
                context.SaveChanges();

                // 4. Set menu availability
                var menuAvailability = new List<MenuAvailability>();

                // Breakfast items only available during breakfast
                foreach (var item in breakfastItems)
                {
                    menuAvailability.Add(new MenuAvailability { ItemID = item.ItemID, SittingType = "Breakfast" });
                }

                // Lunch items available during lunch and dinner
                foreach (var item in lunchItems)
                {
                    menuAvailability.Add(new MenuAvailability { ItemID = item.ItemID, SittingType = "Lunch" });
                    menuAvailability.Add(new MenuAvailability { ItemID = item.ItemID, SittingType = "Dinner" });
                }

                // Dinner items only available during dinner
                foreach (var item in dinnerItems)
                {
                    menuAvailability.Add(new MenuAvailability { ItemID = item.ItemID, SittingType = "Dinner" });
                }

                // Sides, beverages, and desserts available all day
                foreach (var item in sides.Concat(beverages).Concat(desserts))
                {
                    menuAvailability.Add(new MenuAvailability { ItemID = item.ItemID, SittingType = "Breakfast" });
                    menuAvailability.Add(new MenuAvailability { ItemID = item.ItemID, SittingType = "Lunch" });
                    menuAvailability.Add(new MenuAvailability { ItemID = item.ItemID, SittingType = "Dinner" });
                }

                context.MenuAvailability.AddRange(menuAvailability);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error seeding menu data", ex);
            }
        }
    }
}