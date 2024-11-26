using Microsoft.EntityFrameworkCore;
using BeanScene.Models;

namespace BeanScene.Data
{
    public class BeanSceneContext : DbContext
    {
        public BeanSceneContext(DbContextOptions<BeanSceneContext> options)
            : base(options)
        { }

        // RESERVATION SYSTEM
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Sitting> Sittings { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<User> Users { get; set; }

        // ORDERING SYSTEM
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<ItemOption> ItemOptions { get; set; }
        public DbSet<MenuAvailability> MenuAvailability { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // RESERVATION SYSTEM
            modelBuilder.Entity<Reservation>()
                .Property(r => r.ReservationStatus)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            modelBuilder.Entity<Sitting>()
                .Property(s => s.SittingType)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<Table>()
                .Property(t => t.Area)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.UserType)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Unique constraints
            modelBuilder.Entity<Guest>()
                .HasIndex(g => g.Email)
                .IsUnique();

            modelBuilder.Entity<Guest>()
                .HasIndex(g => g.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Relationships
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Guest)
                .WithMany(g => g.Reservations)
                .HasForeignKey(r => r.GuestID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Sitting)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SittingID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Tables)
                .WithMany(t => t.Reservations)
                .UsingEntity(j => j.ToTable("ReservationTables"));

            // ORDERING SYSTEM
            modelBuilder.Entity<MenuAvailability>()
                .HasKey(ma => new { ma.ItemID, ma.SittingType });

            // Configure relationship between OrderItem and ItemOption
            modelBuilder.Entity<OrderItem>()
                .HasMany(oi => oi.SelectedOptions)
                .WithMany(io => io.OrderItems)
                .UsingEntity(j => 
                {
                    j.ToTable("OrderItemOptions");
                    j.Property<int>("OrderItemsOrderItemID");
                    j.Property<int>("SelectedOptionsOptionID");
                });

            // Cascade delete behavior
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuItem>()
                .HasMany(mi => mi.Options)
                .WithOne(io => io.MenuItem)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(mi => mi.OrderItems)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Reservation)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Table)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            // Status fields
            modelBuilder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasConversion<string>()
                .HasDefaultValue("Pending");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.ItemStatus)
                .HasConversion<string>()
                .HasDefaultValue("Pending");
        }
    }
}