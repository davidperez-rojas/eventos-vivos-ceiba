using EventosVivos.API.Models;

namespace EventosVivos.API.DAO.Interfaces;

public interface IReservationDAO
{
    Task<Reservation?> GetByIdAsync(int id);
    Task<IEnumerable<Reservation>> GetByEventAsync(int eventId);
    Task<Reservation> CreateAsync(Reservation reservation);
    Task<Reservation> UpdateAsync(Reservation reservation);
    Task<int> GetConfirmedTicketsAsync(int eventId);
    Task<int> GetReservedTicketsAsync(int eventId);
    Task<bool> ReservationCodeExistsAsync(string code);
    Task<IEnumerable<Reservation>> GetByEventAndEmailAsync(int eventId, string email);
}