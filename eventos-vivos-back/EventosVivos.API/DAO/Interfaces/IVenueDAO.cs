using EventosVivos.API.Models;

namespace EventosVivos.API.DAO.Interfaces;

public interface IVenueDAO
{
    Task<Venue?> GetByIdAsync(int id);
    Task<IEnumerable<Venue>> GetAllAsync();
}