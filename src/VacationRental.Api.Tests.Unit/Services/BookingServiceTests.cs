using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Implementation;
using VacationRental.Api.Services.Models.Booking;
using VacationRental.Api.Tests.Unit.DSL;
using VacationRental.Api.Tests.Unit.Extensions;
using Xunit;

namespace VacationRental.Api.Tests.Unit.Services;

[Collection("Unit")]
public class BookingServiceTests
{
    private readonly IBookingService _bookingService;
    private readonly IBookingRepository _bookingRepository;
    private readonly IRentalRepository _rentalRepository;

    private const int DefaultBookingId = 1;
    private const int DefaultRentalId = 1;
    private readonly DateTime _defaultStartDate = new(2022, 1, 1); 
    private const int DefaultNights = 2;
    
    public BookingServiceTests()
    {
        _bookingRepository = Substitute.For<IBookingRepository>();
        _rentalRepository = Substitute.For<IRentalRepository>();
        _bookingService = new BookingService(_bookingRepository, _rentalRepository, new NullLogger<BookingService>());
    }
    
    [Fact]
    public async Task GetBooking_ReturnsBooking_WhenBookingExists()
    {
        var expectedBooking = Create.Booking().WithId(DefaultBookingId).Please();
        _bookingRepository.GetOrDefaultAsync(DefaultBookingId).Returns(expectedBooking);

        var actualBooking = await _bookingService.GetBookingOrDefaultAsync(DefaultBookingId);
        
        Assert.NotNull(actualBooking);
    }
    
    [Fact]
    public async Task GetBooking_ReturnsCorrectBooking_WhenBookingExists()
    {
        var expectedBooking = Create.Booking().WithId(DefaultBookingId).Please();
        _bookingRepository.GetOrDefaultAsync(DefaultBookingId).Returns(expectedBooking);

        var actualBooking = await _bookingService.GetBookingOrDefaultAsync(DefaultBookingId);
        
        Assert.True(actualBooking!.AreEqual(expectedBooking), "Bookings are not equal");
    }
    
    [Fact]
    public async Task GetBooking_ReturnsNull_WhenBookingNotExists()
    {
        _bookingRepository.GetOrDefaultAsync(DefaultBookingId).ReturnsNull();

        var actualBooking = await _bookingService.GetBookingOrDefaultAsync(DefaultBookingId);
        
        Assert.Null(actualBooking);
    }

    [Fact]
    public async Task CreateBooking_ReturnsIsSuccessFalse_WhenNightsIsNegative()
    {
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, -1);
        
        Assert.False(actualBookingCreationResult.IsSuccess);
    }
    
    [Fact]
    public async Task CreateBooking_ReturnsStatusValidationFailed_WhenNightsIsNegative()
    {
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, -1);
        
        Assert.Equal(CreateBookingResultErrorStatus.ValidationFailed, actualBookingCreationResult.ErrorStatus);
    }
    
    [Fact]
    public async Task CreateBooking_ReturnsIsSuccessFalse_WhenNightsIsZero()
    {
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, 0);
        
        Assert.False(actualBookingCreationResult.IsSuccess);
    }
    
    [Fact]
    public async Task CreateBooking_ReturnsStatusValidationFailed_WhenNightsIsZero()
    {
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, 0);
        
        Assert.Equal(CreateBookingResultErrorStatus.ValidationFailed, actualBookingCreationResult.ErrorStatus);
    }
    
    [Fact]
    public async Task CreateBooking_ReturnsIsSuccessFalse_WhenRentalIsNotExists()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();
        
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.False(actualBookingCreationResult.IsSuccess);
    }
    
    [Fact]
    public async Task CreateBooking_ReturnsStatusValidationFailed_WhenRentalIsNotExists()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();
        
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.Equal(CreateBookingResultErrorStatus.ValidationFailed, actualBookingCreationResult.ErrorStatus);
    }

    [Fact]
    public async Task CreateBooking_ReturnsIsSuccessFalse_WhenAllUnitsInRentalAlreadyBooked()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var booking = Create.Booking().WithRentalId(DefaultRentalId).WithStartDate(_defaultStartDate).WithNights(DefaultNights).Please();
        var bookingArray = new[] {booking};
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                defaultStartDate.AddDays(-rental.PreparationTimeInDays),
                defaultStartDate.AddDays(DefaultNights + rental.PreparationTimeInDays - 1))
            .Returns(bookingArray);
        
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.False(actualBookingCreationResult.IsSuccess);
    }

    [Fact]
    public async Task CreateBooking_ReturnsStatusConflict_WhenAllUnitsInRentalAlreadyBooked()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var booking = Create.Booking().WithRentalId(DefaultRentalId).WithStartDate(_defaultStartDate).WithNights(DefaultNights).Please();
        var bookingArray = new[] {booking};
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                _defaultStartDate.AddDays(-rental.PreparationTimeInDays),
                _defaultStartDate.AddDays(DefaultNights + rental.PreparationTimeInDays - 1))
            .Returns(bookingArray);
        
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.Equal(CreateBookingResultErrorStatus.Conflict, actualBookingCreationResult.ErrorStatus);
    }

    [Fact]
    public async Task CreateBooking_CreateBooking_WhenUnitsInRentalIsAvailable()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(DefaultRentalId).Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                defaultStartDate,
                defaultStartDate.AddDays(DefaultNights - 1))
            .Returns(Array.Empty<Booking>());
        _bookingRepository
            .CreateAsync(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights)
            .Returns(expectedBooking);
        
        await _bookingService.CreateBookingAsync(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights);

        await _bookingRepository
            .Received(1)
            .CreateAsync(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights);
    }
    
    [Fact]
    public async Task CreateBooking_ReturnsIsSuccessTrue_WhenUnitsInRentalIsAvailable()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(DefaultRentalId).Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                defaultStartDate,
                defaultStartDate.AddDays(DefaultNights - 1))
            .Returns(Array.Empty<Booking>());
        _bookingRepository
            .CreateAsync(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights)
            .Returns(expectedBooking);
        
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(
            expectedBooking.RentalId,
            expectedBooking.Start,
            expectedBooking.Nights);

        Assert.True(actualBookingCreationResult.IsSuccess);
    }
    
    [Fact]
    public async Task CreateBooking_ReturnsCorrectBooking_WhenUnitsInRentalIsAvailable()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(DefaultRentalId).WithUnit(1).Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                defaultStartDate,
                defaultStartDate.AddDays(DefaultNights - 1))
            .Returns(Array.Empty<Booking>());
        _bookingRepository
            .CreateAsync(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights)
            .Returns(expectedBooking);
        
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(
            expectedBooking.RentalId,
            expectedBooking.Start,
            expectedBooking.Nights);

        Assert.True(actualBookingCreationResult.CreatedBooking!.AreEqual(expectedBooking), "Bookings are not equal");
    }
}