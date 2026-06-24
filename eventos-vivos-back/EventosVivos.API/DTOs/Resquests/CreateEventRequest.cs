using System.ComponentModel.DataAnnotations;

namespace EventosVivos.API.DTOs.Requests;

public class CreateEventRequest
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 100 caracteres")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 500 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "El venue es obligatorio")]
    public int VenueId { get; set; }

    [Required(ErrorMessage = "La capacidad máxima es obligatoria")]
    [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser un entero positivo")]
    public int MaxCapacity { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    public DateTime StartDateTime { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria")]
    public DateTime EndDateTime { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un decimal positivo")]
    public decimal TicketPrice { get; set; }

    [Required(ErrorMessage = "El tipo de evento es obligatorio")]
    public string EventType { get; set; } = string.Empty;
}