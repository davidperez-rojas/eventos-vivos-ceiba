namespace EventosVivos.API.Models;

public class Reservation
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int Quantity { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public string BuyerEmail { get; set; } = string.Empty;
    public string Status { get; set; } = ReservationStatus.PendingPayment;
    public string? ReservationCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAt { get; set; }
    public bool TicketsLost { get; set; } = false;
    public Event Event { get; set; } = null!;
}

public static class ReservationStatus
{
    public const string PendingPayment = "pendiente_pago";
    public const string Confirmed = "confirmada";
    public const string Cancelled = "cancelada";
}