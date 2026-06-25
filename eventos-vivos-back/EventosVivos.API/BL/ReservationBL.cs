using EventosVivos.API.BL.Interfaces;
using EventosVivos.API.DAO.Interfaces;
using EventosVivos.API.DTOs.Requests;
using EventosVivos.API.DTOs.Responses;
using EventosVivos.API.Models;
using System.Data;

namespace EventosVivos.API.BL;

public class ReservationBL : IReservationBL
{
    private readonly IReservationDAO _reservationDAO;
    private readonly IEventDAO _eventDAO;

    public ReservationBL(IReservationDAO reservationDAO, IEventDAO eventDAO)
    {
        _reservationDAO = reservationDAO;
        _eventDAO = eventDAO;
    }

    public async Task<ReservationResponse> GetByIdAsync(int id)
    {
        var reservation = await _reservationDAO.GetByIdAsync(id);
        if (reservation == null)
            throw new KeyNotFoundException($"No existe una reserva con Id {id}");
        return MapToResponse(reservation);
    }

    public async Task<ReservationResponse> CreateAsync(CreateReservationRequest request)
    {
        var evt = await _eventDAO.GetByIdAsync(request.EventId);
        if (evt == null)
            throw new KeyNotFoundException($"No existe un evento con Id {request.EventId}");

        if (evt.Status != EventStatus.Active)
            throw new InvalidOperationException($"No se pueden hacer reservas para un evento en estado '{evt.Status}'");

        var now = DateTime.UtcNow;
        var minutesToStart = (evt.StartDateTime - now).TotalMinutes;
        var hoursToStart = (evt.StartDateTime - now).TotalHours;

        // RN-04: No reservations within 1 hour of event start
        if (minutesToStart < 60)
            throw new InvalidOperationException("No se permiten reservas para eventos que inicien en menos de 1 hora");

        // RF-03 + RN-05: 24h rule has priority over price rule
        if (hoursToStart < 24)
        {
            if (request.Quantity > 5)
                throw new InvalidOperationException("Para eventos con menos de 24 horas de anticipación, solo se permiten máximo 5 entradas por transacción");
        }
        else if (evt.TicketPrice > 100)
        {
            if (request.Quantity > 10)
                throw new InvalidOperationException("Para eventos con precio mayor a $100, solo se permiten máximo 10 entradas por transacción");
        }

        // RN-01: Validate availability
        var reservedTickets = await _reservationDAO.GetReservedTicketsAsync(request.EventId);
        var available = evt.MaxCapacity - reservedTickets;

        if (request.Quantity > available)
            throw new InvalidOperationException($"No hay suficientes entradas disponibles. Disponibles: {available}");

        var reservation = new Reservation
        {
            EventId = request.EventId,
            Quantity = request.Quantity,
            BuyerName = request.BuyerName.Trim(),
            BuyerEmail = request.BuyerEmail.Trim().ToLower(),
            Status = ReservationStatus.PendingPayment,
            CreatedAt = now
        };

        var created = await _reservationDAO.CreateAsync(reservation);
        var result = await _reservationDAO.GetByIdAsync(created.Id);

        return MapToResponse(result!);
    }

    public async Task<ReservationResponse> ConfirmPaymentAsync(int reservationId)
    {
        var reservation = await _reservationDAO.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new KeyNotFoundException($"No existe una reserva con Id {reservationId}");

        if (reservation.Status == ReservationStatus.Confirmed)
            throw new InvalidOperationException("La reserva ya está confirmada");

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("No se puede confirmar una reserva cancelada");

        reservation.ReservationCode = await GenerateUniqueCodeAsync();
        reservation.Status = ReservationStatus.Confirmed;

        var updated = await _reservationDAO.UpdateAsync(reservation);
        return MapToResponse(updated);
    }

    public async Task<ReservationResponse> CancelAsync(int reservationId)
    {
        var reservation = await _reservationDAO.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new KeyNotFoundException($"No existe una reserva con Id {reservationId}");

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("La reserva ya está cancelada");

        if (reservation.Status == ReservationStatus.PendingPayment)
            throw new InvalidOperationException("Solo se pueden cancelar reservas con estado 'confirmada'");

        var now = DateTime.UtcNow;
        var hoursToEvent = (reservation.Event.StartDateTime - now).TotalHours;

        // RN-07: Penalty if cancelled within 48 hours of event
        reservation.TicketsLost = hoursToEvent < 48;
        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledAt = now;

        var updated = await _reservationDAO.UpdateAsync(reservation);
        return MapToResponse(updated);
    }

    private async Task<string> GenerateUniqueCodeAsync()
    {
        string code;
        do
        {
            var number = Random.Shared.Next(100000, 999999);
            code = $"EV-{number}";
        }
        while (await _reservationDAO.ReservationCodeExistsAsync(code));
        return code;
    }

    private static ReservationResponse MapToResponse(Reservation reservation)
    {
        return new ReservationResponse
        {
            Id = reservation.Id,
            EventId = reservation.EventId,
            EventTitle = reservation.Event?.Title ?? string.Empty,
            Quantity = reservation.Quantity,
            BuyerName = reservation.BuyerName,
            BuyerEmail = reservation.BuyerEmail,
            Status = reservation.Status,
            ReservationCode = reservation.ReservationCode,
            CreatedAt = reservation.CreatedAt,
            CancelledAt = reservation.CancelledAt,
            TicketsLost = reservation.TicketsLost
        };
    }

    public async Task<IEnumerable<ReservationResponse>> GetByEventAsync(int eventId)
    {
        var reservations = await _reservationDAO.GetByEventAsync(eventId);
        return reservations.Select(MapToResponse);
    }
}