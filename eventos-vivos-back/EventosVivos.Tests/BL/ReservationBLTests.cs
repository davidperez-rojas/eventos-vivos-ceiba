using EventosVivos.API.BL;
using EventosVivos.API.DAO.Interfaces;
using EventosVivos.API.DTOs.Requests;
using EventosVivos.API.Models;
using FluentAssertions;
using Moq;

namespace EventosVivos.Tests.BL;

public class ReservationBLTests
{
    private readonly Mock<IReservationDAO> _reservationDAOMock;
    private readonly Mock<IEventDAO> _eventDAOMock;
    private readonly ReservationBL _reservationBL;

    public ReservationBLTests()
    {
        _reservationDAOMock = new Mock<IReservationDAO>();
        _eventDAOMock = new Mock<IEventDAO>();
        _reservationBL = new ReservationBL(_reservationDAOMock.Object, _eventDAOMock.Object);
    }

    private static Event ActiveEvent(decimal price = 50, int capacity = 100) => new()
    {
        Id = 1,
        Title = "Test Event",
        Description = "Test description",
        VenueId = 1,
        MaxCapacity = capacity,
        StartDateTime = DateTime.UtcNow.AddDays(5),
        EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(3),
        TicketPrice = price,
        EventType = EventType.Conference,
        Status = EventStatus.Active,
        Venue = new Venue { Id = 1, Name = "Auditorio", Capacity = 200, City = "Bogotá" },
        Reservations = new List<Reservation>()
    };

    private static CreateReservationRequest ValidRequest(int quantity = 2) => new()
    {
        EventId = 1,
        Quantity = quantity,
        BuyerName = "David Pérez",
        BuyerEmail = "david@test.com"
    };

    private static Reservation ConfirmedReservation() => new()
    {
        Id = 1,
        EventId = 1,
        Quantity = 2,
        BuyerName = "David Pérez",
        BuyerEmail = "david@test.com",
        Status = ReservationStatus.Confirmed,
        ReservationCode = "EV-123456",
        CreatedAt = DateTime.UtcNow.AddHours(-1),
        Event = ActiveEvent()
    };

    [Fact]
    public async Task CreateReservation_WithValidData_ReturnsPendingPaymentReservation()
    {
        var request = ValidRequest();
        var evt = ActiveEvent();
        var created = new Reservation
        {
            Id = 1,
            EventId = 1,
            Quantity = 2,
            BuyerName = "David Pérez",
            BuyerEmail = "david@test.com",
            Status = ReservationStatus.PendingPayment,
            CreatedAt = DateTime.UtcNow,
            Event = evt
        };

        _eventDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(evt);
        _reservationDAOMock.Setup(d => d.GetReservedTicketsAsync(1)).ReturnsAsync(0);
        _reservationDAOMock.Setup(d => d.CreateAsync(It.IsAny<Reservation>())).ReturnsAsync(created);
        _reservationDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(created);

        var result = await _reservationBL.CreateAsync(request);

        result.Should().NotBeNull();
        result.Status.Should().Be(ReservationStatus.PendingPayment);
        result.Quantity.Should().Be(2);
    }

    [Fact]
    public async Task CreateReservation_EventNotFound_ThrowsKeyNotFoundException()
    {
        _eventDAOMock.Setup(d => d.GetByIdAsync(99)).ReturnsAsync((Event?)null);
        var request = ValidRequest();
        request.EventId = 99;

        var action = async () => await _reservationBL.CreateAsync(request);

        await action.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*No existe un evento*");
    }

    [Fact]
    public async Task CreateReservation_EventNotActive_ThrowsInvalidOperationException()
    {
        var evt = ActiveEvent();
        evt.Status = EventStatus.Cancelled;
        _eventDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(evt);

        var action = async () => await _reservationBL.CreateAsync(ValidRequest());

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*estado*");
    }

    [Fact]
    public async Task CreateReservation_LessThan1HourToStart_ThrowsInvalidOperationException()
    {
        var evt = ActiveEvent();
        evt.StartDateTime = DateTime.UtcNow.AddMinutes(30);
        _eventDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(evt);

        var action = async () => await _reservationBL.CreateAsync(ValidRequest());

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*menos de 1 hora*");
    }

    [Fact]
    public async Task CreateReservation_PriceOver100_MoreThan10Tickets_ThrowsInvalidOperationException()
    {
        var evt = ActiveEvent(price: 150);
        _eventDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(evt);
        _reservationDAOMock.Setup(d => d.GetReservedTicketsAsync(1)).ReturnsAsync(0);

        var action = async () => await _reservationBL.CreateAsync(ValidRequest(quantity: 11));

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*máximo 10 entradas*");
    }

    [Fact]
    public async Task CreateReservation_Within24Hours_HighPrice_Limits5Tickets()
    {
        var evt = ActiveEvent(price: 150);
        evt.StartDateTime = DateTime.UtcNow.AddHours(10);
        _eventDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(evt);
        _reservationDAOMock.Setup(d => d.GetReservedTicketsAsync(1)).ReturnsAsync(0);

        var action = async () => await _reservationBL.CreateAsync(ValidRequest(quantity: 6));

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*24 horas*");
    }

    [Fact]
    public async Task CreateReservation_NoAvailability_ThrowsInvalidOperationException()
    {
        var evt = ActiveEvent(capacity: 10);
        _eventDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(evt);
        _reservationDAOMock.Setup(d => d.GetReservedTicketsAsync(1)).ReturnsAsync(10);

        var action = async () => await _reservationBL.CreateAsync(ValidRequest(quantity: 1));

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*suficientes entradas*");
    }

    [Fact]
    public async Task ConfirmPayment_ValidReservation_ChangesStatusAndGeneratesCode()
    {
        var reservation = new Reservation
        {
            Id = 1,
            EventId = 1,
            Quantity = 2,
            BuyerName = "David",
            BuyerEmail = "david@test.com",
            Status = ReservationStatus.PendingPayment,
            CreatedAt = DateTime.UtcNow,
            Event = ActiveEvent()
        };

        _reservationDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(reservation);
        _reservationDAOMock.Setup(d => d.ReservationCodeExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _reservationDAOMock.Setup(d => d.UpdateAsync(It.IsAny<Reservation>())).ReturnsAsync((Reservation r) => r);

        var result = await _reservationBL.ConfirmPaymentAsync(1);

        result.Status.Should().Be(ReservationStatus.Confirmed);
        result.ReservationCode.Should().MatchRegex(@"^EV-\d{6}$");
    }

    [Fact]
    public async Task ConfirmPayment_AlreadyConfirmed_ThrowsInvalidOperationException()
    {
        _reservationDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(ConfirmedReservation());

        var action = async () => await _reservationBL.ConfirmPaymentAsync(1);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ya está confirmada*");
    }

    [Fact]
    public async Task ConfirmPayment_CancelledReservation_ThrowsInvalidOperationException()
    {
        var reservation = ConfirmedReservation();
        reservation.Status = ReservationStatus.Cancelled;
        _reservationDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(reservation);

        var action = async () => await _reservationBL.ConfirmPaymentAsync(1);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*cancelada*");
    }

    [Fact]
    public async Task CancelReservation_Confirmed_ChangesStatusToCancelled()
    {
        var reservation = ConfirmedReservation();
        reservation.Event.StartDateTime = DateTime.UtcNow.AddDays(10);

        _reservationDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(reservation);
        _reservationDAOMock.Setup(d => d.UpdateAsync(It.IsAny<Reservation>())).ReturnsAsync((Reservation r) => r);

        var result = await _reservationBL.CancelAsync(1);

        result.Status.Should().Be(ReservationStatus.Cancelled);
        result.TicketsLost.Should().BeFalse();
        result.CancelledAt.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelReservation_Within48Hours_MarksTicketsAsLost()
    {
        var reservation = ConfirmedReservation();
        reservation.Event.StartDateTime = DateTime.UtcNow.AddHours(10);

        _reservationDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(reservation);
        _reservationDAOMock.Setup(d => d.UpdateAsync(It.IsAny<Reservation>())).ReturnsAsync((Reservation r) => r);

        var result = await _reservationBL.CancelAsync(1);

        result.Status.Should().Be(ReservationStatus.Cancelled);
        result.TicketsLost.Should().BeTrue();
    }

    [Fact]
    public async Task CancelReservation_PendingPayment_ThrowsInvalidOperationException()
    {
        var reservation = ConfirmedReservation();
        reservation.Status = ReservationStatus.PendingPayment;
        _reservationDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(reservation);

        var action = async () => await _reservationBL.CancelAsync(1);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*confirmada*");
    }

    [Fact]
    public async Task CancelReservation_AlreadyCancelled_ThrowsInvalidOperationException()
    {
        var reservation = ConfirmedReservation();
        reservation.Status = ReservationStatus.Cancelled;
        _reservationDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(reservation);

        var action = async () => await _reservationBL.CancelAsync(1);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ya está cancelada*");
    }
}