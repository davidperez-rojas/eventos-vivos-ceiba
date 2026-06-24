namespace EventosVivos.API.DTOs.Responses;

public class EventResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public VenueResponse Venue { get; set; } = null!;
    public int MaxCapacity { get; set; }
    public int AvailableTickets { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public decimal TicketPrice { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}