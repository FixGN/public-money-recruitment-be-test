using System;
using NSubstitute;
using NUnit.Framework;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Implementation;
using VacationRental.Api.Services.Models;
using VacationRental.Api.Tests.Unit.DSL;
using VacationRental.Api.Tests.Unit.Extensions;

namespace VacationRental.Api.Tests.Unit.Services;

public class BookingServiceTests
{
    private readonly IBookingService _bookingService;
    private readonly IBookingRepository _bookingRepository;
    private readonly IRentalRepository _rentalRepository;

    private const int DefaultBookingId = 1;
    private const int DefaultRentalId = 1;
    private readonly DateTime _defaultStartDate = new DateTime(2022, 1, 1); 
    private const int DefaultNights = 2;
    
    public BookingServiceTests()
    {
        _bookingRepository = Substitute.For<IBookingRepository>();
        _rentalRepository = Substitute.For<IRentalRepository>();
        _bookingService = new BookingService(_bookingRepository, _rentalRepository);
    }
    
    [Test]
    public void GetBooking_ReturnsBooking_WhenBookingExists()
    {
        var expectedBooking = Create.Booking().WithId(DefaultBookingId).Please();
        _bookingRepository.Get(DefaultBookingId).Returns(expectedBooking);

        var actualBooking = _bookingService.GetBooking(DefaultBookingId);
        
        Assert.NotNull(actualBooking);
    }
    
    [Test]
    public void GetBooking_ReturnsCorrectBooking_WhenBookingExists()
    {
        var expectedBooking = Create.Booking().WithId(DefaultBookingId).Please();
        _bookingRepository.Get(DefaultBookingId).Returns(expectedBooking);

        var actualBooking = _bookingService.GetBooking(DefaultBookingId);
        
        Assert.IsTrue(actualBooking!.AreEqual(expectedBooking));
    }
    
    [Test]
    public void GetBooking_ReturnsNull_WhenBookingNotExists()
    {
        _bookingRepository.Get(DefaultBookingId).Returns((Booking?) null);

        var actualBooking = _bookingService.GetBooking(DefaultBookingId);
        
        Assert.Null(actualBooking);
    }

    [Test]
    public void CreateBooking_ReturnsIsSuccessFalse_WhenNightsIsNegative()
    {
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, -1);
        
        Assert.AreEqual(false, actualBookingCreationResult.IsSuccess);
    }
    
    [Test]
    public void CreateBooking_ReturnsStatusValidationFailed_WhenNightsIsNegative()
    {
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, -1);
        
        Assert.AreEqual(CreateBookingResultStatus.ValidationFailed, actualBookingCreationResult.Status);
    }
    
    [Test]
    public void CreateBooking_ReturnsIsSuccessFalse_WhenNightsIsZero()
    {
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, 0);
        
        Assert.AreEqual(false, actualBookingCreationResult.IsSuccess);
    }
    
    [Test]
    public void CreateBooking_ReturnsStatusValidationFailed_WhenNightsIsZero()
    {
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, 0);
        
        Assert.AreEqual(CreateBookingResultStatus.ValidationFailed, actualBookingCreationResult.Status);
    }
    
    [Test]
    public void CreateBooking_ReturnsIsSuccessFalse_WhenRentalIsNotExists()
    {
        _rentalRepository.Get(DefaultRentalId).Returns((Rental?) null);
        
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.AreEqual(false, actualBookingCreationResult.IsSuccess);
    }
    
    [Test]
    public void CreateBooking_ReturnsStatusValidationFailed_WhenRentalIsNotExists()
    {
        _rentalRepository.Get(DefaultRentalId).Returns((Rental?) null);
        
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.AreEqual(CreateBookingResultStatus.ValidationFailed, actualBookingCreationResult.Status);
    }

    [Test]
    public void CreateBooking_ReturnsIsSuccessFalse_WhenAllUnitsInRentalAlreadyBooked()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var booking = Create.Booking().WithRentalId(DefaultRentalId).WithStartDate(_defaultStartDate).WithNights(DefaultNights).Please();
        var bookingArray = new[] {booking};
        _rentalRepository.Get(DefaultRentalId).Returns(rental);
        _bookingRepository.GetByRentalId(DefaultRentalId).Returns(bookingArray);
        
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.AreEqual(false, actualBookingCreationResult.IsSuccess);
    }

    [Test]
    public void CreateBooking_ReturnsStatusConflict_WhenAllUnitsInRentalAlreadyBooked()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var booking = Create.Booking().WithRentalId(DefaultRentalId).WithStartDate(_defaultStartDate).WithNights(DefaultNights).Please();
        var bookingArray = new[] {booking};
        _rentalRepository.Get(DefaultRentalId).Returns(rental);
        _bookingRepository.GetByRentalId(DefaultRentalId).Returns(bookingArray);
        
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.AreEqual(CreateBookingResultStatus.Conflict, actualBookingCreationResult.Status);
    }

    [Test]
    public void CreateBooking_CreateBooking_WhenUnitsInRentalIsAvailable()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(DefaultRentalId).Please();
        _rentalRepository.Get(DefaultRentalId).Returns(rental);
        _bookingRepository.GetByRentalId(DefaultRentalId).Returns(Array.Empty<Booking>());
        _bookingRepository.Create(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights).Returns(expectedBooking);
        
        _bookingService.CreateBooking(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights);

        _bookingRepository.Received(1).Create(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights);
    }
    
    [Test]
    public void CreateBooking_ReturnsIsSuccessTrue_WhenUnitsInRentalIsAvailable()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(DefaultRentalId).Please();
        _rentalRepository.Get(DefaultRentalId).Returns(rental);
        _bookingRepository.GetByRentalId(DefaultRentalId).Returns(Array.Empty<Booking>());
        _bookingRepository.Create(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights).Returns(expectedBooking);
        
        var actualBookingCreationResult = _bookingService.CreateBooking(
            expectedBooking.RentalId,
            expectedBooking.Start,
            expectedBooking.Nights);

        Assert.AreEqual(true, actualBookingCreationResult.IsSuccess);
    }
    
    [Test]
    public void CreateBooking_ReturnsCorrectBooking_WhenUnitsInRentalIsAvailable()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(DefaultRentalId).Please();
        _rentalRepository.Get(DefaultRentalId).Returns(rental);
        _bookingRepository.GetByRentalId(DefaultRentalId).Returns(Array.Empty<Booking>());
        _bookingRepository.Create(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights).Returns(expectedBooking);
        
        var actualBookingCreationResult = _bookingService.CreateBooking(
            expectedBooking.RentalId,
            expectedBooking.Start,
            expectedBooking.Nights);

        Assert.IsTrue(actualBookingCreationResult.CreatedBooking!.AreEqual(expectedBooking));
    }
}