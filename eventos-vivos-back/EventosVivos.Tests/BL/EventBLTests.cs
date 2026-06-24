using EventosVivos.API.BL;
using EventosVivos.API.DAO.Interfaces;
using EventosVivos.API.DTOs.Requests;
using EventosVivos.API.Models;
using FluentAssertions;
using Moq;

namespace EventosVivos.Tests.BL;

public class EventBLTests
{
    private readonly Mock<IEventDAO> _eventDAOMock;
    private readonly Mock<IVenueDAO> _venueDAOMock;
    private readonly EventBL _eventBL;

    public EventBLTests()
    {
        _eventDAOMock = new Mock<IEventDAO>();
        _venueDAOMock = new Mock<IVenueDAO>();
        _eventBL = new EventBL(_eventDAOMock.Object, _venueDAOMock.Object);
    }

    private static Venue VenueBase() => new()
    {
        Id = 1,
        Name = "Auditorio Central",
        Capacity = 200,
        City = "Bogotá"
    };

    private static CreateEventRequest ValidRequest() => new()
    {
        Title = "Tech Conference 2025",
        Description = "A great conference about technology and innovation",
        VenueId = 1,
        MaxCapacity = 100,
        StartDateTime = DateTime.UtcNow.AddDays(5),
        EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(3),
        TicketPrice = 50,
        EventType = "conferencia"
    };

    private Event EventBase(CreateEventRequest? req = null)
    {
        req ??= ValidRequest();
        return new Event
        {
            Id = 1,
            Title = req.Title,
            Description = req.Description,
            VenueId = req.VenueId,
            MaxCapacity = req.MaxCapacity,
            StartDateTime = req.StartDateTime,
            EndDateTime = req.EndDateTime,
            TicketPrice = req.TicketPrice,
            EventType = req.EventType,
            Status = EventStatus.Active,
            Venue = VenueBase(),
            Reservations = new List<Reservation>()
        };
    }

    [Fact]
    public async Task CreateEvent_WithValidData_ReturnsCreatedEvent()
    {
        var request = ValidRequest();
        var created = EventBase(request);

        _venueDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(VenueBase());
        _eventDAOMock.Setup(d => d.HasOverlapAsync(request.VenueId, request.StartDateTime, request.EndDateTime, null)).ReturnsAsync(false);
        _eventDAOMock.Setup(d => d.CreateAsync(It.IsAny<Event>())).ReturnsAsync(created);
        _eventDAOMock.Setup(d => d.GetByIdAsync(created.Id)).ReturnsAsync(created);
        _eventDAOMock.Setup(d => d.GetActiveEventsAsync()).ReturnsAsync(new List<Event>());

        var result = await _eventBL.CreateAsync(request);

        result.Should().NotBeNull();
        result.Title.Should().Be(request.Title);
        result.Status.Should().Be(EventStatus.Active);
    }

    [Fact]
    public async Task CreateEvent_WithInvalidType_ThrowsArgumentException()
    {
        var request = ValidRequest();
        request.EventType = "festival";

        _venueDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(VenueBase());
        _eventDAOMock.Setup(d => d.GetActiveEventsAsync()).ReturnsAsync(new List<Event>());

        var action = async () => await _eventBL.CreateAsync(request);

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Tipo de evento inválido*");
    }

    [Fact]
    public async Task CreateEvent_WithNonExistentVenue_ThrowsKeyNotFoundException()
    {
        var request = ValidRequest();
        _venueDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync((Venue?)null);

        var action = async () => await _eventBL.CreateAsync(request);

        await action.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*No existe un venue*");
    }

    [Fact]
    public async Task CreateEvent_WithVenueOverlap_ThrowsInvalidOperationException()
    {
        var request = ValidRequest();
        _venueDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(VenueBase());
        _eventDAOMock.Setup(d => d.GetActiveEventsAsync()).ReturnsAsync(new List<Event>());
        _eventDAOMock.Setup(d => d.HasOverlapAsync(request.VenueId, request.StartDateTime, request.EndDateTime, null)).ReturnsAsync(true);

        var action = async () => await _eventBL.CreateAsync(request);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*venue ya tiene un evento*");
    }

    [Fact]
    public async Task UpdateStatuses_EventPastEndTime_MarksAsCompleted()
    {
        var expiredEvent = EventBase();
        expiredEvent.EndDateTime = DateTime.UtcNow.AddHours(-1);

        _eventDAOMock.Setup(d => d.GetActiveEventsAsync()).ReturnsAsync(new List<Event> { expiredEvent });
        _eventDAOMock.Setup(d => d.UpdateAsync(It.IsAny<Event>())).ReturnsAsync((Event e) => e);

        await _eventBL.UpdateStatusesAsync();

        _eventDAOMock.Verify(d => d.UpdateAsync(
            It.Is<Event>(e => e.Status == EventStatus.Completed)), Times.Once);
    }
}