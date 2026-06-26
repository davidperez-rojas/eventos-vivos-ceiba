using EventosVivos.API.DTOs.Requests;
using EventosVivos.API.DTOs.Responses;

namespace EventosVivos.API.BL.Interfaces;

public interface IEventBL
{
    Task<IEnumerable<EventResponse>> GetAllAsync(EventFilterRequest filters);
    Task<EventResponse> GetByIdAsync(int id);
    Task<EventResponse> CreateAsync(CreateEventRequest request);
    Task UpdateStatusesAsync();
    Task<OccupancyReportResponse> GetReportAsync(int eventId);
    Task<EventResponse> CancelEventAsync(int eventId);
}