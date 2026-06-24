namespace EventosVivos.API.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int VenueId { get; set; }
    public int MaxCapacity { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public decimal TicketPrice { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Status { get; set; } = EventStatus.Active;
    public Venue Venue { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

public static class EventStatus
{
    public const string Active = "activo";
    public const string Cancelled = "cancelado";
    public const string Completed = "completado";
}

public static class EventType
{
    public const string Conference = "conferencia";
    public const string Workshop = "taller";
    public const string Concert = "concierto";
    public static readonly string[] Valid = { Conference, Workshop, Concert };
}