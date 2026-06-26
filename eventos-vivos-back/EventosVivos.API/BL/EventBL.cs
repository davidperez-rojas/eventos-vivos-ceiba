using EventosVivos.API.BL.Interfaces;
using EventosVivos.API.DAO.Interfaces;
using EventosVivos.API.DTOs.Requests;
using EventosVivos.API.DTOs.Responses;
using EventosVivos.API.Models;

namespace EventosVivos.API.BL;

public class EventBL : IEventBL
{
    private readonly IEventDAO _eventDAO;
    private readonly IVenueDAO _venueDAO;

    public EventBL(IEventDAO eventDAO, IVenueDAO venueDAO)
    {
        _eventDAO = eventDAO;
        _venueDAO = venueDAO;
    }

    public async Task<IEnumerable<EventResponse>> GetAllAsync(EventFilterRequest filters)
    {
        await UpdateStatusesAsync();
        var events = await _eventDAO.GetAllAsync(
            filters.Type, filters.DateFrom, filters.DateTo,
            filters.VenueId, filters.Status, filters.Title);
        return events.Select(MapToResponse);
    }

    public async Task<EventResponse> GetByIdAsync(int id)
    {
        await UpdateStatusesAsync();
        var evt = await _eventDAO.GetByIdAsync(id);
        if (evt == null)
            throw new KeyNotFoundException($"No existe un evento con Id {id}");
        return MapToResponse(evt);
    }

    public async Task<EventResponse> CreateAsync(CreateEventRequest request)
    {
        // Validate event type
        if (!EventType.Valid.Contains(request.EventType.ToLower()))
            throw new ArgumentException($"Tipo de evento inválido. Valores permitidos: {string.Join(", ", EventType.Valid)}");

        // Validate venue exists — RN-01
        var venue = await _venueDAO.GetByIdAsync(request.VenueId);
        if (venue == null)
            throw new KeyNotFoundException($"No existe un venue con Id {request.VenueId}");

        // RN-02: No venue overlap
        var hasOverlap = await _eventDAO.HasOverlapAsync(
            request.VenueId, request.StartDateTime, request.EndDateTime);
        if (hasOverlap)
            throw new InvalidOperationException("El venue ya tiene un evento activo en ese horario");

        var evt = new Event
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            VenueId = request.VenueId,
            MaxCapacity = request.MaxCapacity,
            StartDateTime = request.StartDateTime,
            EndDateTime = request.EndDateTime,
            TicketPrice = request.TicketPrice,
            EventType = request.EventType.ToLower(),
            Status = EventStatus.Active
        };

        var created = await _eventDAO.CreateAsync(evt);
        var result = await _eventDAO.GetByIdAsync(created.Id);
        return MapToResponse(result!);
    }

    // RN-06: Auto-complete events past their end time
    public async Task UpdateStatusesAsync()
    {
        var activeEvents = await _eventDAO.GetActiveEventsAsync();
        var now = DateTime.UtcNow;

        foreach (var evt in activeEvents)
        {
            if (evt.EndDateTime < now)
            {
                evt.Status = EventStatus.Completed;
                await _eventDAO.UpdateAsync(evt);
            }
        }
    }

    public async Task<OccupancyReportResponse> GetReportAsync(int eventId)
    {
        var evt = await _eventDAO.GetByIdAsync(eventId);
        if (evt == null)
            throw new KeyNotFoundException($"No existe un evento con Id {eventId}");

        var reservations = evt.Reservations.ToList();

        var ticketsSold = reservations
            .Where(r => r.Status == ReservationStatus.Confirmed)
            .Sum(r => r.Quantity);

        var ticketsLost = reservations
            .Where(r => r.TicketsLost)
            .Sum(r => r.Quantity);

        var ticketsOccupied = reservations
            .Where(r => r.Status == ReservationStatus.Confirmed
                     || r.Status == ReservationStatus.PendingPayment)
            .Sum(r => r.Quantity);

        var available = evt.MaxCapacity - ticketsOccupied - ticketsLost;

        var percentage = evt.MaxCapacity > 0
            ? Math.Round((decimal)ticketsSold / evt.MaxCapacity * 100, 2)
            : 0;

        var totalRevenue = reservations
            .Where(r => r.Status == ReservationStatus.Confirmed)
            .Sum(r => r.Quantity * evt.TicketPrice);

        return new OccupancyReportResponse
        {
            EventId = evt.Id,
            EventTitle = evt.Title,
            MaxCapacity = evt.MaxCapacity,
            TicketsSold = ticketsSold,
            AvailableTickets = available,
            OccupancyPercentage = percentage,
            TotalRevenue = totalRevenue,
            EventStatus = evt.Status
        };
    }

    private static EventResponse MapToResponse(Event evt)
    {
        var occupied = evt.Reservations
            .Where(r => r.Status == ReservationStatus.Confirmed
                     || r.Status == ReservationStatus.PendingPayment)
            .Sum(r => r.Quantity);

        return new EventResponse
        {
            Id = evt.Id,
            Title = evt.Title,
            Description = evt.Description,
            Venue = new VenueResponse
            {
                Id = evt.Venue.Id,
                Name = evt.Venue.Name,
                Capacity = evt.Venue.Capacity,
                City = evt.Venue.City
            },
            MaxCapacity = evt.MaxCapacity,
            AvailableTickets = evt.MaxCapacity - occupied,
            StartDateTime = evt.StartDateTime,
            EndDateTime = evt.EndDateTime,
            TicketPrice = evt.TicketPrice,
            EventType = evt.EventType,
            Status = evt.Status
        };
    }

    public async Task<EventResponse> CancelEventAsync(int eventId)
    {
        await UpdateStatusesAsync();
        var evt = await _eventDAO.GetByIdAsync(eventId);

        if (evt == null)
            throw new KeyNotFoundException($"No se ha encontrado el evento {eventId} para cancelar.");

        evt.Status = EventStatus.Cancelled;
        await _eventDAO.UpdateAsync(evt);

        return MapToResponse(evt);
    }
}