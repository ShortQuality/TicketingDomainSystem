using Microsoft.EntityFrameworkCore;
using TicketingSystem.DAL.Entities;

namespace TicketingSystem.DAL
{
    public class TicketingSystemContext : DbContext
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails{ get; set; }
        public DbSet<Event> Events{ get; set; }
        public DbSet<Payment> Payments{ get; set; }
        public DbSet<Price> Prices{ get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<SeatRow> SeatRows{ get; set; }
        public DbSet<Section> Sections{ get; set; }
        public DbSet<Venue> Venues{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.CartDetails);
            });

            modelBuilder.Entity<CartDetail>(entity =>
            {
                entity.HasKey(e => e.CartId);
                entity.Property(p => p.EventId);
                entity.HasMany(e => e.Seats);
                entity.HasOne(d => d.Cart)
                  .WithMany(p => p.CartDetails);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Venue);
                entity.Property(p => p.Name);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Status);
                entity.Property(p => p.CartId);
                entity.HasOne(d => d.Cart);

            });

            modelBuilder.Entity<Price>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Amount);
                entity.HasMany(d => d.Seats);
            });

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Number);
                entity.Property(p => p.Price);
                entity.HasOne(d => d.SeatRow);
            });

            modelBuilder.Entity<SeatRow>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Number);
                entity.HasMany(d => d.Seats);
                entity.HasOne(d => d.Section).WithMany(d => d.SeatRows);
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Letter);
                entity.Property(p => p.Number);
                entity.HasMany(d => d.SeatRows);
                entity.HasOne(d => d.Venue).WithMany(d => d.Sections);
            });

            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Name);
                entity.HasMany(d => d.Sections);
                entity.HasMany(d => d.Events).WithOne(d => d.Venue);
            });
        }
    }
}