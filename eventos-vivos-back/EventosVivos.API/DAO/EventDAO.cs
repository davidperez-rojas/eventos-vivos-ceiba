using EventosVivos.API.Data;
using EventosVivos.API.DAO.Interfaces;
using EventosVivos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.API.DAO;

public class EventDAO : IEventDAO
{
    private readonly AppDbContext _context;

    public EventDAO(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Event>> GetAllAsync(string? type, DateTime? dateFrom,
        DateTime? dateTo, int? venueId, string? status, string? title)
    {
        var query = _context.Events
            .Include(e => e.Venue)
            .Include(e => e.Reservations)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(e => e.EventType == type.ToLower());

        if (dateFrom.HasValue)
            query = query.Where(e => e.StartDateTime >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(e => e.StartDateTime <= dateTo.Value);

        if (venueId.HasValue)
            query = query.Where(e => e.VenueId == venueId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status == status.ToLower());

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(e => e.Title.ToLower().Contains(title.ToLower()));

        return await query.OrderBy(e => e.StartDateTime).ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _context.Events
            .Include(e => e.Venue)
            .Include(e => e.Reservations)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Event> CreateAsync(Event evt)
    {
        _context.Events.Add(evt);
        await _context.SaveChangesAsync();
        return evt;
    }

    public async Task<Event> UpdateAsync(Event evt)
    {
        _context.Events.Update(evt);
        await _context.SaveChangesAsync();
        return evt;
    }

    public async Task<bool> HasOverlapAsync(int venueId, DateTime start, DateTime end,
        int? excludeEventId = null)
    {
        var query = _context.Events
            .Where(e => e.VenueId == venueId
                && e.Status != EventStatus.Cancelled
                && e.StartDateTime < end
                && e.EndDateTime > start);

        if (excludeEventId.HasValue)
            query = query.Where(e => e.Id != excludeEventId.Value);

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Event>> GetActiveEventsAsync()
    {
        return await _context.Events
            .Where(e => e.Status == EventStatus.Active)
            .ToListAsync();
    }
}