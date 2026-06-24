using EventosVivos.API.Data;
using EventosVivos.API.DAO.Interfaces;
using EventosVivos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.API.DAO;

public class VenueDAO : IVenueDAO
{
    private readonly AppDbContext _context;

    public VenueDAO(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Venue?> GetByIdAsync(int id)
    {
        return await _context.Venues.FindAsync(id);
    }

    public async Task<IEnumerable<Venue>> GetAllAsync()
    {
        return await _context.Venues.ToListAsync();
    }
}