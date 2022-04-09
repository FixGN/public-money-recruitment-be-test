using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Implementation;
using VacationRental.Api.Services.Interfaces;
using VacationRental.Api.Services.Models.Rental;
using VacationRental.Api.Tests.Unit.DSL;
using VacationRental.Api.Tests.Unit.Extensions;
using Xunit;

namespace VacationRental.Api.Tests.Unit.Services.Rental;

[Collection("Unit")]
public class UpdateRentalTests
{
    private const int DefaultRentalId = 1;
    private const int DefaultUnits = 1;
    private const int DefaultPreparationTimeInDays = 1;

    private readonly IBookingRepository _bookingRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IRentalService _rentalService;

    public UpdateRentalTests()
    {
        _rentalRepository = Substitute.For<IRentalRepository>();
        _bookingRepository = Substitute.For<IBookingRepository>();
        _rentalService = new RentalService(_rentalRepository, _bookingRepository, new NullLogger<RentalService>());
    }

    [Fact]
    public async Task GivenNoRental_WhenUnitsIsNegativeNumber_ThenReturnsIsSuccessFalse()
    {
        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, -1, DefaultPreparationTimeInDays);

        Assert.False(actualResult.IsSuccess);
    }

    [Fact]
    public async Task GivenNoRental_WhenUnitsIsNegativeNumber_ThenReturnsErrorStatusValidationFailed()
    {
        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, -1, DefaultPreparationTimeInDays);

        Assert.Equal(UpdateRentalResultErrorStatus.ValidationFailed, actualResult.ResultErrorStatus);
    }

    [Fact]
    public async Task GivenNoRental_WhenPreparationTimeInDaysIsNegativeNumber_ThenReturnsIsSuccessFalse()
    {
        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, DefaultUnits, -1);

        Assert.False(actualResult.IsSuccess);
    }

    [Fact]
    public async Task GivenNoRental_WhenPreparationTimeInDaysIsNegativeNumber_ThenReturnsErrorStatusValidationFailed()
    {
        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, DefaultUnits, -1);

        Assert.Equal(UpdateRentalResultErrorStatus.ValidationFailed, actualResult.ResultErrorStatus);
    }

    [Fact]
    public async Task GivenNoRental_WhenRentalNotExists_ThenReturnsIsSuccessFalse()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();

        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, DefaultUnits, DefaultPreparationTimeInDays);

        Assert.False(actualResult.IsSuccess);
    }

    [Fact]
    public async Task GivenNoRental_WhenRentalNotExists_ThenReturnsErrorStatusRentalNotFound()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();

        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, DefaultUnits, DefaultPreparationTimeInDays);

        Assert.Equal(UpdateRentalResultErrorStatus.RentalNotFound, actualResult.ResultErrorStatus);
    }

    [Fact]
    public async Task GivenRentalExists_WhenExistingRentalHasSameUnitsAndPreparationTimeInDaysValues_ThenReturnsIsSuccessTrue()
    {
        var existingRental = Create.Rental().Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);

        var actualResult = await _rentalService.UpdateRentalAsync(
            existingRental.Id,
            existingRental.Units,
            existingRental.PreparationTimeInDays);

        Assert.True(actualResult.IsSuccess);
    }

    [Fact]
    public async Task
        GivenRentalExistsWithNoBookings_WhenExistingRentalHasSameUnitsAndPreparationTimeInDaysValues_ThenReturnsCorrectRentalValue()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.GetOrDefaultAsync(expectedRental.Id).Returns(expectedRental);

        var actualResult = await _rentalService.UpdateRentalAsync(
            expectedRental.Id,
            expectedRental.Units,
            expectedRental.PreparationTimeInDays);

        Assert.True(expectedRental.AreEqual(actualResult.Rental!), "Rentals are not equal");
    }

    [Fact]
    public async Task GivenRentalExistsWithNoBookings_WhenAllParamsIsCorrect_ThenReturnsIsSuccessTrue()
    {
        var existingRental = Create.Rental().Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);

        var actualResult = await _rentalService.UpdateRentalAsync(
            existingRental.Id,
            existingRental.Units + 1,
            existingRental.PreparationTimeInDays);

        Assert.True(actualResult.IsSuccess);
    }

    [Fact]
    public async Task GivenRentalExistsWithNoBookings_WhenAllParamsIsCorrect_ThenReturnsCorrectRental()
    {
        var existingRental = Create.Rental().Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);

        var expectedRental = Create.Rental()
            .WithId(existingRental.Id)
            .WithUnits(existingRental.Units + 1)
            .WithPreparationTimeInDays(existingRental.PreparationTimeInDays)
            .WithVersion(existingRental.Version + 1)
            .Please();


        var actualResult = await _rentalService.UpdateRentalAsync(
            existingRental.Id,
            expectedRental.Units,
            expectedRental.PreparationTimeInDays);

        Assert.True(expectedRental.AreEqual(actualResult.Rental!), "Rentals are not equal");
    }

    [Fact]
    public async Task GivenRentalExistsWithBookings_WhenPreparationTimeMakesConflictBetweenCreatedBookings_ThenReturnsIsSuccessFalse()
    {
        var existingRental = Create.Rental().WithUnits(1).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);
        var booking1 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(1)
            .WithStartDate(new DateTime(2022, 1, 1))
            .WithNights(1)
            .Please();
        var booking2 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(1)
            .WithStartDate(new DateTime(2022, 1, 4))
            .WithNights(1)
            .Please();
        var existingBookings = new[] {booking1, booking2};
        _bookingRepository.GetByRentalIdAsync(existingRental.Id).Returns(existingBookings);

        var actualResult = await _rentalService.UpdateRentalAsync(
            existingRental.Id,
            existingRental.Units,
            existingRental.PreparationTimeInDays + 1);

        Assert.False(actualResult.IsSuccess);
    }

    [Fact]
    public async Task GivenRentalExistsWithBookings_WhenPreparationTimeMakesConflictBetweenCreatedBookings_ThenReturnsErrorStatusConflict()
    {
        var existingRental = Create.Rental().WithUnits(1).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);
        var booking1 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(1)
            .WithStartDate(new DateTime(2022, 1, 1))
            .WithNights(1)
            .Please();
        var booking2 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(1)
            .WithStartDate(new DateTime(2022, 1, 4))
            .WithNights(1)
            .Please();
        var existingBookings = new[] {booking1, booking2};
        _bookingRepository.GetByRentalIdAsync(existingRental.Id).Returns(existingBookings);

        var actualResult = await _rentalService.UpdateRentalAsync(
            existingRental.Id,
            existingRental.Units,
            existingRental.PreparationTimeInDays + 1);

        Assert.Equal(UpdateRentalResultErrorStatus.Conflict, actualResult.ResultErrorStatus);
    }

    [Fact]
    public async Task GivenRentalExistsWithBookings_WhenPreparationTimeMakesConflictBetweenCurrentBookingsCountAndNewUnitsCount_ThenReturnsIsSuccessFalse()
    {
        var existingRental = Create.Rental().WithUnits(3).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);
        var booking1 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(1)
            .WithStartDate(new DateTime(2022, 1, 1))
            .WithNights(2)
            .Please();
        var booking2 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(2)
            .WithStartDate(new DateTime(2022, 1, 2))
            .WithNights(2)
            .Please();
        var booking3 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(3)
            .WithStartDate(new DateTime(2022, 1, 3))
            .WithNights(2)
            .Please();
        var existingBookings = new[] {booking1, booking2, booking3};
        _bookingRepository.GetByRentalIdAsync(existingRental.Id).Returns(existingBookings);

        var actualResult = await _rentalService.UpdateRentalAsync(
            existingRental.Id,
            existingRental.Units - 1,
            existingRental.PreparationTimeInDays);

        Assert.False(actualResult.IsSuccess);
    }

    [Fact]
    public async Task GivenRentalExistsWithBookings_WhenPreparationTimeMakesConflictBetweenCurrentBookingsCountAndNewUnitsCount_ThenReturnsErrorStatusConflict()
    {
        var existingRental = Create.Rental().WithUnits(3).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);
        var booking1 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(1)
            .WithStartDate(new DateTime(2022, 1, 1))
            .WithNights(2)
            .Please();
        var booking2 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(2)
            .WithStartDate(new DateTime(2022, 1, 2))
            .WithNights(2)
            .Please();
        var booking3 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(3)
            .WithStartDate(new DateTime(2022, 1, 3))
            .WithNights(2)
            .Please();
        var existingBookings = new[] {booking1, booking2, booking3};
        _bookingRepository.GetByRentalIdAsync(existingRental.Id).Returns(existingBookings);

        var actualResult = await _rentalService.UpdateRentalAsync(
            existingRental.Id,
            existingRental.Units - 1,
            existingRental.PreparationTimeInDays);

        Assert.Equal(UpdateRentalResultErrorStatus.Conflict, actualResult.ResultErrorStatus);
    }

    [Fact]
    public async Task GivenRentalExistsWithBookings_WhenAllParamsAreCorrectAndConflictsNotFound_ThenUpdatesRentalUnitsValueInRepository()
    {
        var existingRental = Create.Rental().WithUnits(3).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);
        var booking1 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(1)
            .WithStartDate(new DateTime(2022, 1, 1))
            .WithNights(2)
            .Please();
        var booking2 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(2)
            .WithStartDate(new DateTime(2022, 1, 2))
            .WithNights(2)
            .Please();
        var existingBookings = new[] {booking1, booking2};
        _bookingRepository.GetByRentalIdAsync(existingRental.Id).Returns(existingBookings);

        await _rentalService.UpdateRentalAsync(
            existingRental.Id,
            existingRental.Units - 1,
            existingRental.PreparationTimeInDays + 1);

        await _rentalRepository.Received(1).UpdateAsync(Arg.Any<Models.Rental>());
    }

    [Fact]
    public async Task GivenRentalExistsWithBookings_WhenAllParamsAreCorrectAndConflictsNotFound_ThenUpdatesRentalUnitsValueInRepositoryWithNewUnitsAndPreparationTimeInDaysValues()
    {
        var existingRental = Create.Rental().WithUnits(3).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);
        var booking1 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(1)
            .WithStartDate(new DateTime(2022, 1, 1))
            .WithNights(2)
            .Please();
        var booking2 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(2)
            .WithStartDate(new DateTime(2022, 1, 2))
            .WithNights(2)
            .Please();
        var existingBookings = new[] {booking1, booking2};
        _bookingRepository.GetByRentalIdAsync(existingRental.Id).Returns(existingBookings);
        var expectedRentalToUpdate = Create.Rental()
            .WithId(existingRental.Id)
            .WithUnits(existingRental.Units - 1)
            .WithPreparationTimeInDays(existingRental.PreparationTimeInDays + 1)
            .Please();

        await _rentalService.UpdateRentalAsync(
            existingRental.Id,
            expectedRentalToUpdate.Units,
            expectedRentalToUpdate.PreparationTimeInDays);

        await _rentalRepository
            .Received(1)
            .UpdateAsync(Arg.Is<Models.Rental>(x =>
                x.Id == expectedRentalToUpdate.Id
                && x.Units == expectedRentalToUpdate.Units
                && x.PreparationTimeInDays == expectedRentalToUpdate.PreparationTimeInDays));
    }

    [Fact]
    public async Task GivenRentalExistsWithBookings_WhenAllParamsAreCorrectAndConflictsNotFound_ThenReturnsIsSuccessTrue()
    {
        var existingRental = Create.Rental().WithUnits(3).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);
        var booking1 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(1)
            .WithStartDate(new DateTime(2022, 1, 1))
            .WithNights(2)
            .Please();
        var booking2 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(2)
            .WithStartDate(new DateTime(2022, 1, 2))
            .WithNights(2)
            .Please();
        var existingBookings = new[] {booking1, booking2};
        _bookingRepository.GetByRentalIdAsync(existingRental.Id).Returns(existingBookings);

        var actualResult = await _rentalService.UpdateRentalAsync(
            existingRental.Id,
            existingRental.Units - 1,
            existingRental.PreparationTimeInDays + 1);

        Assert.True(actualResult.IsSuccess);
    }

    [Fact]
    public async Task GivenRentalExistsWithBookings_WhenAllParamsAreCorrectAndConflictsNotFound_ThenReturnsCorrectRentalValue()
    {
        var existingRental = Create.Rental().WithUnits(3).WithPreparationTimeInDays(2).WithVersion(2).Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);

        var booking1 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(1)
            .WithStartDate(new DateTime(2022, 1, 1))
            .WithNights(2)
            .Please();
        var booking2 = Create.Booking()
            .WithRentalId(existingRental.Id)
            .WithUnit(2)
            .WithStartDate(new DateTime(2022, 1, 2))
            .WithNights(2)
            .Please();
        var existingBookings = new[] {booking1, booking2};
        _bookingRepository.GetByRentalIdAsync(existingRental.Id).Returns(existingBookings);

        var expectedRental = Create.Rental()
            .WithId(existingRental.Id)
            .WithUnits(existingRental.Units - 1)
            .WithPreparationTimeInDays(existingRental.PreparationTimeInDays + 1)
            .WithVersion(existingRental.Version + 1)
            .Please();

        var actualResult = await _rentalService.UpdateRentalAsync(
            existingRental.Id,
            existingRental.Units - 1,
            existingRental.PreparationTimeInDays + 1);

        Assert.True(actualResult.Rental!.AreEqual(expectedRental), "Rentals are not equal");
    }
}