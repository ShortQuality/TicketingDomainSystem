using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using TicketingSystem.DAL.Entities;

namespace TicketingSystem.DAL
{
    public class TicketingSystemContext : DbContext
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Event> Events{ get; set; }
        public DbSet<Payment> Payments{ get; set; }
        public DbSet<Price> Prices{ get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<SeatRow> SeatRows { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Cart).WithOne(e => e.User).HasForeignKey<Cart>(e => e.Id);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).IsRequired().HasMaxLength(500);
                entity.Property(p => p.Date).IsRequired();
                entity.HasOne(e => e.Venue).WithOne(e => e.Event).HasForeignKey<Venue>(e => e.Id);
            });

            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Address).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Letter).IsRequired();
                entity.Property(p => p.Number).IsRequired();
                entity.HasOne(e => e.Venue).WithMany(v => v.Sections).HasForeignKey(e => e.VenueId);
            });

            modelBuilder.Entity<SeatRow>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Number);
                entity.HasOne(e => e.Section).WithMany(e => e.SeatRows).HasForeignKey(e => e.SectionId);
            });

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.SeatState);
                entity.Property(p => p.Number);
                entity.HasOne(p => p.SeatRow).WithMany(e => e.Seats).HasForeignKey(e => e.SeatRowId);
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.PurchaseDate).IsRequired();
                entity.HasOne(e => e.Event).WithMany(e => e.Tickets).HasForeignKey(e => e.EventId);
                entity.HasOne(e => e.User).WithMany(e => e.Tickets).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Seat).WithMany(e => e.Tickets).HasForeignKey(e => e.SeatId);
                entity.HasOne(e => e.Price).WithMany(e => e.Tickets).HasForeignKey(e => e.PriceId);
                entity.HasOne(e => e.Cart).WithMany(e => e.Tickets).HasForeignKey(e => e.Cart);
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.TotalAmount);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Status);
                entity.HasOne(e => e.User).WithOne(e => e.Payment).HasForeignKey<Payment>(e => e.UserId);
            });
        }
    }
}