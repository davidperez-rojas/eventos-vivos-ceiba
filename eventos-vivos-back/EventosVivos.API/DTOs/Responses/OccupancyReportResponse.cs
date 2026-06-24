namespace EventosVivos.API.DTOs.Responses;

public class OccupancyReportResponse
{
    public int EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int TicketsSold { get; set; }
    public int AvailableTickets { get; set; }
    public decimal OccupancyPercentage { get; set; }
    public decimal TotalRevenue { get; set; }
    public string EventStatus { get; set; } = string.Empty;
}