using EventosVivos.API.BL.Interfaces;
using EventosVivos.API.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EventosVivos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationBL _reservationBL;

    public ReservationsController(IReservationBL reservationBL)
    {
        _reservationBL = reservationBL;
    }

    // POST /api/reservations
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequest request)
    {
        var reservation = await _reservationBL.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    // GET /api/reservations/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var reservation = await _reservationBL.GetByIdAsync(id);
        return Ok(reservation);
    }

    // PUT /api/reservations/{id}/confirm
    [HttpPut("{id:int}/confirm")]
    public async Task<IActionResult> ConfirmPayment(int id)
    {
        var reservation = await _reservationBL.ConfirmPaymentAsync(id);
        return Ok(reservation);
    }

    // PUT /api/reservations/{id}/cancel
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var reservation = await _reservationBL.CancelAsync(id);
        return Ok(reservation);
    }

    // GET /api/reservations/event/{eventId}
    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> GetByEvent(int eventId, [FromQuery] string? email = null)
    {
        var reservations = await _reservationBL.GetByEventAsync(eventId);
        return Ok(reservations);
    }
}