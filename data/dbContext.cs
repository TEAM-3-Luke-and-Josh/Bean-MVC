using Microsoft.EntityFrameworkCore;
using BeanScene.Models;

namespace BeanScene.Data
{
    public class BeanSceneContext : DbContext
    {
        public BeanSceneContext(DbContextOptions<BeanSceneContext> options)
            : base(options)
        { }

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Sitting> Sittings { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure enum-like string constraints
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

            // Configure unique constraints
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

            // Configure relationships
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
        }
    }
}