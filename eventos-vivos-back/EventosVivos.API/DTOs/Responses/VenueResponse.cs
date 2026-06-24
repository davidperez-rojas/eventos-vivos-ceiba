namespace EventosVivos.API.DTOs.Responses;

public class VenueResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string City { get; set; } = string.Empty;
}