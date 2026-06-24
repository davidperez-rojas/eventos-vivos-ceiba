namespace EventosVivos.API.DTOs.Requests;

public class EventFilterRequest
{
    public string? Type { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int? VenueId { get; set; }
    public string? Status { get; set; }
    public string? Title { get; set; }
}