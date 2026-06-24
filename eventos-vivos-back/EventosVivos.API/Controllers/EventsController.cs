using EventosVivos.API.BL.Interfaces;
using EventosVivos.API.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EventosVivos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventBL _eventBL;

    public EventsController(IEventBL eventBL)
    {
        _eventBL = eventBL;
    }

    // GET /api/events
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? type,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] int? venueId,
        [FromQuery] string? status,
        [FromQuery] string? title)
    {
        var filters = new EventFilterRequest
        {
            Type = type,
            DateFrom = dateFrom,
            DateTo = dateTo,
            VenueId = venueId,
            Status = status,
            Title = title
        };
        var events = await _eventBL.GetAllAsync(filters);
        return Ok(events);
    }

    // GET /api/events/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var evt = await _eventBL.GetByIdAsync(id);
        return Ok(evt);
    }

    // POST /api/events
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
    {
        var evt = await _eventBL.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = evt.Id }, evt);
    }

    // GET /api/events/{id}/report
    [HttpGet("{id:int}/report")]
    public async Task<IActionResult> GetReport(int id)
    {
        var report = await _eventBL.GetReportAsync(id);
        return Ok(report);
    }
}