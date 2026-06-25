using EventosVivos.API.DTOs.Requests;
using EventosVivos.API.DTOs.Responses;

namespace EventosVivos.API.BL.Interfaces;

public interface IReservationBL
{
    Task<ReservationResponse> CreateAsync(CreateReservationRequest request);
    Task<ReservationResponse> GetByIdAsync(int id);
    Task<ReservationResponse> ConfirmPaymentAsync(int reservationId);
    Task<ReservationResponse> CancelAsync(int reservationId);
    Task<IEnumerable<ReservationResponse>> GetByEventAsync(int eventId, string? email = null);
}