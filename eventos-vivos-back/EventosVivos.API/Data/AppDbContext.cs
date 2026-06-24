using EventosVivos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Name).IsRequired().HasMaxLength(100);
            entity.Property(v => v.City).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.EventType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TicketPrice).HasPrecision(18, 2);
            entity.HasOne(e => e.Venue)
                  .WithMany(v => v.Events)
                  .HasForeignKey(e => e.VenueId);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.BuyerName).IsRequired().HasMaxLength(100);
            entity.Property(r => r.BuyerEmail).IsRequired().HasMaxLength(150);
            entity.Property(r => r.Status).IsRequired().HasMaxLength(20);
            entity.Property(r => r.ReservationCode).HasMaxLength(10);
            entity.HasOne(r => r.Event)
                  .WithMany(e => e.Reservations)
                  .HasForeignKey(r => r.EventId);
        });

        modelBuilder.Entity<Venue>().HasData(
            new Venue { Id = 1, Name = "Auditorio Central", Capacity = 200, City = "Bogotá" },
            new Venue { Id = 2, Name = "Sala Norte", Capacity = 50, City = "Bogotá" },
            new Venue { Id = 3, Name = "Arena Sur", Capacity = 500, City = "Medellín" }
        );
    }
}