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
                _defaultStartDate.AddDays(-rental.PreparationTimeInDays),
                _defaultStartDate.AddDays(DefaultNights + rental.PreparationTimeInDays - 1))
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
                _defaultStartDate.AddDays(-rental.PreparationTimeInDays),
                _defaultStartDate.AddDays(DefaultNights + rental.PreparationTimeInDays - 1))
            .Returns(bookingArray);

        var actualBookingCreationResult = await _bookingService.CreateBookingAsync(DefaultRentalId, _defaultStartDate, DefaultNights);

        Assert.Equal(CreateBookingResultErrorStatus.Conflict, actualBookingCreationResult.ErrorStatus);
    }

    [Fact]
    public async Task GivenRentalExistsAndUnitsIsAvailableForBooking_WhenCreateBooking_ThenCreatesBookingInRepository()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).WithUnits(1).Please();
        var expectedBooking = Create.Booking().WithRentalId(DefaultRentalId).Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                _defaultStartDate,
                _defaultStartDate.AddDays(DefaultNights - 1))
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
        var expectedBooking = Create.Booking().WithRentalId(DefaultRentalId).Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                _defaultStartDate,
                _defaultStartDate.AddDays(DefaultNights - 1))
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
        var expectedBooking = Create.Booking().WithRentalId(DefaultRentalId).WithUnit(1).Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                _defaultStartDate,
                _defaultStartDate.AddDays(DefaultNights - 1))
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
}