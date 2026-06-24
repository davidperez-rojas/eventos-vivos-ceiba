using System.ComponentModel.DataAnnotations;

namespace EventosVivos.API.DTOs.Requests;

public class CreateReservationRequest
{
    [Required(ErrorMessage = "El ID del evento es obligatorio")]
    public int EventId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "El nombre del comprador es obligatorio")]
    public string BuyerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email del comprador es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    public string BuyerEmail { get; set; } = string.Empty;
}