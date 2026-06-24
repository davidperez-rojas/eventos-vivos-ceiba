using EventosVivos.API.Models;

namespace EventosVivos.API.DAO.Interfaces;

public interface IEventDAO
{
    Task<IEnumerable<Event>> GetAllAsync(string? type, DateTime? dateFrom,
        DateTime? dateTo, int? venueId, string? status, string? title);
    Task<Event?> GetByIdAsync(int id);
    Task<Event> CreateAsync(Event evt);
    Task<Event> UpdateAsync(Event evt);
    Task<bool> HasOverlapAsync(int venueId, DateTime start, DateTime end, int? excludeEventId = null);
    Task<IEnumerable<Event>> GetActiveEventsAsync();
}