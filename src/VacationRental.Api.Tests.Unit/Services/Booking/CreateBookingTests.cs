using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Implementation;
using VacationRental.Api.Services.Interfaces;
using VacationRental.Api.Services.Models.Booking;
using VacationRental.Api.Tests.Unit.DSL;
using VacationRental.Api.Tests.Unit.Extensions;
using Xunit;

namespace VacationRental.Api.Tests.Unit.Services.Booking;

[Collection("Unit")]
public class CreateBookingTests
{
    private const int DefaultRentalId = 1;
    private const int DefaultNights = 2;
    private readonly DateOnly _defaultStartDate = new(2022, 1, 1);

    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingService _bookingService;
    private readonly IRentalRepository _rentalRepository;

    public CreateBookingTests()
    {
        _bookingRepository = Substitute.For<IBookingRepository>();
        _rentalRepository = Substitute.For<IRentalRepository>();
        _bookingService = new BookingService(_bookingRepository, _rentalRepository, new NullLogger<BookingService>());
    }

    [Fact]
    public async Task GivenNoBookings_WhenNightsIsNegative_ThenReturnsIsSuccessFalse()
    {
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, -1);

        Assert.False(actualBookingCreationResult.IsSuccess);
    }

    [Fact]
    public async Task GivenNoBookings_WhenNightsIsNegative_ThenReturnsStatusValidationFailed()
    {
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, -1);

        Assert.Equal(CreateBookingResultErrorStatus.ValidationFailed, actualBookingCreationResult.ErrorStatus);
    }

    [Fact]
    public async Task GivenNoBookings_WhenNightsIsZero_ThenReturnsIsSuccessFalse()
    {
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, 0);

        Assert.False(actualBookingCreationResult.IsSuccess);
    }

    [Fact]
    public async Task GivenNoBookings_WhenNightsIsZero_ThenReturnsStatusValidationFailed()
    {
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, 0);

        Assert.Equal(CreateBookingResultErrorStatus.ValidationFailed, actualBookingCreationResult.ErrorStatus);
    }

    [Fact]
    public async Task GivenRentalDoesNotCreated_WhenRentalIsNotExists_ThenReturnsIsSuccessFalse()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();

        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, DefaultNights);

        Assert.False(actualBookingCreationResult.IsSuccess);
    }

    [Fact]
    public async Task GivenRentalDoesNotCreated_WhenRentalIsNotExists_ThenReturnsStatusValidationFailed()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();

        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, DefaultNights);

        Assert.Equal(CreateBookingResultErrorStatus.ValidationFailed, actualBookingCreationResult.ErrorStatus);
    }

    [Fact]
    public async Task GivenRentalExistsAndAllUnitsAlreadyBooked_WhenCreateBooking_ThenReturnsIsSuccessFalse()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var booking = Create.Booking().WithRentalId(DefaultRentalId).WithStartDate(_defaultStartDate).WithNights(DefaultNights).Please();
        var bookingArray = new[] {booking};
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                CalculateStartDateForGetByRentalIdAndDatePeriodAsync(_defaultStartDate, rental.PreparationTimeInDays),
                CalculateEndDate(_defaultStartDate, DefaultNights, rental.PreparationTimeInDays))
            .Returns(bookingArray);

        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, DefaultNights);

        Assert.False(actualBookingCreationResult.IsSuccess);
    }

    [Fact]
    public async Task GivenRentalExistsAndAllUnitsAlreadyBooked_WhenCreateBooking_ThenReturnsStatusConflict()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var booking = Create.Booking().WithRentalId(DefaultRentalId).WithStartDate(_defaultStartDate).WithNights(DefaultNights).Please();
        var bookingArray = new[] {booking};
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                CalculateStartDateForGetByRentalIdAndDatePeriodAsync(_defaultStartDate, rental.PreparationTimeInDays),
                CalculateEndDate(booking.Start, booking.Nights, rental.PreparationTimeInDays))
            .Returns(bookingArray);

        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, DefaultNights);

        Assert.Equal(CreateBookingResultErrorStatus.Conflict, actualBookingCreationResult.ErrorStatus);
    }

    [Fact]
    public async Task GivenRentalExistsAndUnitsIsAvailableForBooking_WhenCreateBooking_ThenCreatesBookingInRepository()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(rental.Id).Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                CalculateStartDateForGetByRentalIdAndDatePeriodAsync(expectedBooking.Start, rental.PreparationTimeInDays),
                CalculateEndDate(_defaultStartDate, DefaultNights, rental.PreparationTimeInDays))
            .Returns(Array.Empty<Models.Booking>());
        _bookingRepository
            .CreateAsync(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights)
            .Returns(expectedBooking);

        await _bookingService.CreateBookingAsync(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights);

        await _bookingRepository
            .Received(1)
            .CreateAsync(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights);
    }

    [Fact]
    public async Task GivenRentalExistsAndUnitsIsAvailableForBooking_WhenCreateBooking_ThenReturnsIsSuccessTrue()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(rental.Id).Please();
        _rentalRepository.GetOrDefaultAsync(rental.Id).Returns(rental);
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                CalculateStartDateForGetByRentalIdAndDatePeriodAsync(expectedBooking.Start, rental.PreparationTimeInDays),
                CalculateEndDate(expectedBooking.Start, expectedBooking.Nights, rental.PreparationTimeInDays))
            .Returns(Array.Empty<Models.Booking>());
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
    public async Task GivenRentalExistsAndUnitsIsAvailableForBooking_WhenCreateBooking_ThenReturnsCorrectBooking()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(rental.Id).WithUnit(1).Please();
        _rentalRepository.GetOrDefaultAsync(rental.Id).Returns(rental);
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                CalculateStartDateForGetByRentalIdAndDatePeriodAsync(expectedBooking.Start, rental.PreparationTimeInDays),
                CalculateEndDate(expectedBooking.Start, expectedBooking.Nights, rental.PreparationTimeInDays))
            .Returns(Array.Empty<Models.Booking>());
        _bookingRepository
            .CreateAsync(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights)
            .Returns(expectedBooking);

        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(
            expectedBooking.RentalId,
            expectedBooking.Start,
            expectedBooking.Nights);

        Assert.True(actualBookingCreationResult.CreatedBooking!.AreEqual(expectedBooking), "Bookings are not equal");
    }

    [Fact]
    public async Task GivenRentalExistsAndHasTwoUnitsAndOneBookedUnits_WhenCreateBooking_ThenReturnsIsSuccessTrue()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(2).WithPreparationTimeInDays(1).Please();
        var createdBooking1 = Create.Booking()
            .WithRentalId(rental.Id)
            .WithUnit(1)
            .WithStartDate(new DateOnly(2022, 1, 1))
            .WithNights(2)
            .Please();
        var createdBooking2 = Create.Booking()
            .WithRentalId(rental.Id)
            .WithUnit(1)
            .WithStartDate(new DateOnly(2022, 1, 4))
            .WithNights(2)
            .Please();
        _rentalRepository.GetOrDefaultAsync(rental.Id).Returns(rental);
        
        var expectedBooking = Create.Booking()
            .WithRentalId(rental.Id)
            .WithUnit(2)
            .WithStartDate(new DateOnly(2022, 1, 1))
            .WithNights(6)
            .Please();
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                rental.Id,
                CalculateStartDateForGetByRentalIdAndDatePeriodAsync(expectedBooking.Start, rental.PreparationTimeInDays),
                CalculateEndDate(expectedBooking.Start, expectedBooking.Nights, rental.PreparationTimeInDays))
            .Returns(new [] { createdBooking1, createdBooking2 });
        
        _bookingRepository
            .CreateAsync(expectedBooking.RentalId, expectedBooking.Start, expectedBooking.Nights)
            .Returns(expectedBooking);

        
        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(
            expectedBooking.RentalId,
            expectedBooking.Start,
            expectedBooking.Nights);

        Assert.True(actualBookingCreationResult.IsSuccess);
    }
    
    private static DateOnly CalculateEndDate(DateOnly startDate, int nights, int preparationTimeInDays)
    {
        return startDate.AddDays(nights + preparationTimeInDays - 1);
    }
    
    private static DateOnly CalculateStartDateForGetByRentalIdAndDatePeriodAsync(DateOnly startDate, int preparationTimeInDays)
    {
        return startDate.AddDays(-preparationTimeInDays);
    }
}