using EventosVivos.API.Data;
using EventosVivos.API.DAO.Interfaces;
using EventosVivos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.API.DAO;

public class ReservationDAO : IReservationDAO
{
    private readonly AppDbContext _context;

    public ReservationDAO(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation?> GetByIdAsync(int id)
    {
        return await _context.Reservations
            .Include(r => r.Event)
            .ThenInclude(e => e.Venue)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Reservation>> GetByEventAsync(int eventId)
    {
        return await _context.Reservations
            .Where(r => r.EventId == eventId)
            .ToListAsync();
    }

    public async Task<Reservation> CreateAsync(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        return reservation;
    }

    public async Task<Reservation> UpdateAsync(Reservation reservation)
    {
        _context.Reservations.Update(reservation);
        await _context.SaveChangesAsync();
        return reservation;
    }

    public async Task<int> GetConfirmedTicketsAsync(int eventId)
    {
        return await _context.Reservations
            .Where(r => r.EventId == eventId && r.Status == ReservationStatus.Confirmed)
            .SumAsync(r => (int?)r.Quantity) ?? 0;
    }

    public async Task<int> GetReservedTicketsAsync(int eventId)
    {
        return await _context.Reservations
            .Where(r => r.EventId == eventId
                && (r.Status == ReservationStatus.Confirmed
                    || r.Status == ReservationStatus.PendingPayment))
            .SumAsync(r => (int?)r.Quantity) ?? 0;
    }

    public async Task<bool> ReservationCodeExistsAsync(string code)
    {
        return await _context.Reservations
            .AnyAsync(r => r.ReservationCode == code);
    }
}