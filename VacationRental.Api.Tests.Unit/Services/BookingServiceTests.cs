using System;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
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
        _bookingService = new BookingService(_bookingRepository, _rentalRepository, new NullLogger<BookingService>());
    }
    
    [Test]
    public void GetBooking_ReturnsBooking_WhenBookingExists()
    {
        var expectedBooking = Create.Booking().WithId(DefaultBookingId).Please();
        _bookingRepository.GetOrDefault(DefaultBookingId).Returns(expectedBooking);

        var actualBooking = _bookingService.GetBookingOrDefault(DefaultBookingId);
        
        Assert.NotNull(actualBooking);
    }
    
    [Test]
    public void GetBooking_ReturnsCorrectBooking_WhenBookingExists()
    {
        var expectedBooking = Create.Booking().WithId(DefaultBookingId).Please();
        _bookingRepository.GetOrDefault(DefaultBookingId).Returns(expectedBooking);

        var actualBooking = _bookingService.GetBookingOrDefault(DefaultBookingId);
        
        Assert.IsTrue(actualBooking!.AreEqual(expectedBooking));
    }
    
    [Test]
    public void GetBooking_ReturnsNull_WhenBookingNotExists()
    {
        _bookingRepository.GetOrDefault(DefaultBookingId).ReturnsNull();

        var actualBooking = _bookingService.GetBookingOrDefault(DefaultBookingId);
        
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
        _rentalRepository.GetOrDefault(DefaultRentalId).ReturnsNull();
        
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.AreEqual(false, actualBookingCreationResult.IsSuccess);
    }
    
    [Test]
    public void CreateBooking_ReturnsStatusValidationFailed_WhenRentalIsNotExists()
    {
        _rentalRepository.GetOrDefault(DefaultRentalId).ReturnsNull();
        
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.AreEqual(CreateBookingResultStatus.ValidationFailed, actualBookingCreationResult.Status);
    }

    [Test]
    public void CreateBooking_ReturnsIsSuccessFalse_WhenAllUnitsInRentalAlreadyBooked()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var booking = Create.Booking().WithRentalId(DefaultRentalId).WithStartDate(_defaultStartDate).WithNights(DefaultNights).Please();
        var bookingArray = new[] {booking};
        _rentalRepository.GetOrDefault(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriod(
                DefaultRentalId,
                defaultStartDate,
                defaultStartDate.AddDays(DefaultNights - 1))
            .Returns(bookingArray);
        
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.AreEqual(false, actualBookingCreationResult.IsSuccess);
    }

    [Test]
    public void CreateBooking_ReturnsStatusConflict_WhenAllUnitsInRentalAlreadyBooked()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var booking = Create.Booking().WithRentalId(DefaultRentalId).WithStartDate(_defaultStartDate).WithNights(DefaultNights).Please();
        var bookingArray = new[] {booking};
        _rentalRepository.GetOrDefault(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriod(
                DefaultRentalId,
                defaultStartDate,
                defaultStartDate.AddDays(DefaultNights - 1))
            .Returns(bookingArray);
        
        var actualBookingCreationResult = _bookingService.CreateBooking(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.AreEqual(CreateBookingResultStatus.Conflict, actualBookingCreationResult.Status);
    }

    [Test]
    public void CreateBooking_CreateBooking_WhenUnitsInRentalIsAvailable()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(DefaultRentalId).Please();
        _rentalRepository.GetOrDefault(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriod(
                DefaultRentalId,
                defaultStartDate,
                defaultStartDate.AddDays(DefaultNights - 1))
            .Returns(Array.Empty<Booking>());
        _bookingRepository.Create(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights).Returns(expectedBooking);
        
        _bookingService.CreateBooking(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights);

        _bookingRepository.Received(1).Create(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights);
    }
    
    [Test]
    public void CreateBooking_ReturnsIsSuccessTrue_WhenUnitsInRentalIsAvailable()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(DefaultRentalId).Please();
        _rentalRepository.GetOrDefault(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriod(
                DefaultRentalId,
                defaultStartDate,
                defaultStartDate.AddDays(DefaultNights - 1))
            .Returns(Array.Empty<Booking>());
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
        _rentalRepository.GetOrDefault(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriod(
                DefaultRentalId,
                defaultStartDate,
                defaultStartDate.AddDays(DefaultNights - 1))
            .Returns(Array.Empty<Booking>());
        _bookingRepository.Create(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights).Returns(expectedBooking);
        
        var actualBookingCreationResult = _bookingService.CreateBooking(
            expectedBooking.RentalId,
            expectedBooking.Start,
            expectedBooking.Nights);

        Assert.IsTrue(actualBookingCreationResult.CreatedBooking!.AreEqual(expectedBooking));
    }
}