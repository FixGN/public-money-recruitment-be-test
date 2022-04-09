using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Implementation;
using VacationRental.Api.Services.Models.Rental;
using VacationRental.Api.Tests.Unit.DSL;
using VacationRental.Api.Tests.Unit.Extensions;

namespace VacationRental.Api.Tests.Unit.Services;

public class RentalServiceTests
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IRentalService _rentalService;
    private readonly IBookingRepository _bookingRepository;

    private const int DefaultRentalId = 1;
    private const int DefaultUnits = 1;
    private const int DefaultPreparationTimeInDays = 1;

    public RentalServiceTests()
    {
        _rentalRepository = Substitute.For<IRentalRepository>();
        _bookingRepository = Substitute.For<IBookingRepository>();
        _rentalService = new RentalService(_rentalRepository, _bookingRepository);
    }
    
    [SetUp]
    public void Setup()
    {
        _rentalRepository.ClearReceivedCalls();
        _bookingRepository.ClearReceivedCalls();
        _rentalRepository.ClearReceivedCalls();
    }
    
    [Test]
    public async Task GetRental_ReturnsNull_WhenRentalNotExists()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();

        var actualRental = await _rentalService.GetRentalOrDefaultAsync(DefaultRentalId);
        
        Assert.IsNull(actualRental);
    }

    [Test]
    public async Task GetRental_ReturnsRental_WhenRentalExists()
    {
        var rental = Create.Rental().Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);

        var actualRental = await _rentalService.GetRentalOrDefaultAsync(DefaultRentalId);
        
        Assert.IsInstanceOf<Rental>(actualRental);
    }
    
    [Test]
    public async Task GetRental_ReturnsCorrectRental_WhenRentalExists()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(expectedRental);

        var actualRental = await _rentalService.GetRentalOrDefaultAsync(DefaultRentalId);
        
        Assert.IsTrue(actualRental!.AreEqual(expectedRental));
    }

    [Test]
    public async Task CreateRental_ReturnsIsSuccessFalse_WhenUnitsIsNegativeNumber()
    {
        var actualRentalResult = await _rentalService.CreateRentalAsync(-1, DefaultPreparationTimeInDays);
        
        Assert.IsFalse(actualRentalResult.IsSuccess);
    }

    [Test]
    public async Task CreateRental_ReturnsErrorStatusValidationFail_WhenUnitsIsNegativeNumber()
    {
        var actualRentalResult = await _rentalService.CreateRentalAsync(-1, DefaultPreparationTimeInDays);
        
        Assert.AreEqual(CreateRentalResultErrorStatus.ValidationFailed, actualRentalResult.ErrorStatus);
    }
    
    [Test]
    public async Task CreateRental_ReturnsIsSuccessFalse_WhenPreparationTimeInDaysIsNegativeNumber()
    {
        var actualRentalResult = await _rentalService.CreateRentalAsync(DefaultUnits, -1);
        
        Assert.IsFalse(actualRentalResult.IsSuccess);
    }

    [Test]
    public async Task CreateRental_ReturnsErrorStatusValidationFail_WhenPreparationTimeInDaysIsNegativeNumber()
    {
        var actualRentalResult = await _rentalService.CreateRentalAsync(DefaultUnits, -1);
        
        Assert.AreEqual(CreateRentalResultErrorStatus.ValidationFailed, actualRentalResult.ErrorStatus);
    }
    
    [Test]
    public async Task CreateRental_ReturnsIsSuccessTrue_WhenAllParametersIsCorrect()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.CreateAsync(expectedRental.Units, expectedRental.PreparationTimeInDays).Returns(expectedRental);

        var actualRentalResult = await _rentalService.CreateRentalAsync(expectedRental.Units, expectedRental.PreparationTimeInDays);
        
        Assert.IsTrue(actualRentalResult.IsSuccess);
    }
    
    [Test]
    public async Task CreateRental_ReturnsCorrectRental_WhenAllParametersIsCorrect()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.CreateAsync(expectedRental.Units, expectedRental.PreparationTimeInDays).Returns(expectedRental);

        var actualRentalResult = await _rentalService.CreateRentalAsync(expectedRental.Units, expectedRental.PreparationTimeInDays);
        
        Assert.IsTrue(actualRentalResult.Rental!.AreEqual(expectedRental));
    }

    [Test]
    public async Task UpdateRental_ReturnsIsSuccessFalse_WhenUnitsIsNegativeNumber()
    {
        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, -1, DefaultPreparationTimeInDays);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public async Task UpdateRental_ReturnsErrorStatusValidationFailed_WhenUnitsIsNegativeNumber()
    {
        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, -1, DefaultPreparationTimeInDays);
        
        Assert.AreEqual(UpdateRentalResultErrorStatus.ValidationFailed, actualResult.ResultErrorStatus);
    }
    
    [Test]
    public async Task UpdateRental_ReturnsIsSuccessFalse_WhenPreparationTimeInDaysIsNegativeNumber()
    {
        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, DefaultUnits, -1);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public async Task UpdateRental_ReturnsErrorStatusValidationFailed_WhenPreparationTimeInDaysIsNegativeNumber()
    {
        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, DefaultUnits, -1);
        
        Assert.AreEqual(UpdateRentalResultErrorStatus.ValidationFailed, actualResult.ResultErrorStatus);
    }
    
    [Test]
    public async Task UpdateRental_ReturnsIsSuccessFalse_WhenRentalNotExists()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();
        
        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, DefaultUnits, DefaultPreparationTimeInDays);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public async Task UpdateRental_ReturnsErrorStatusRentalNotFound_WhenRentalNotExists()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();
        
        var actualResult = await _rentalService.UpdateRentalAsync(DefaultRentalId, DefaultUnits, DefaultPreparationTimeInDays);
        
        Assert.AreEqual(UpdateRentalResultErrorStatus.RentalNotFound, actualResult.ResultErrorStatus);
    }

    [Test]
    public async Task UpdateRental_ReturnsIsSuccessTrue_WhenExistingRentalHasSameUnitsAndPreparationTimeInDaysValues()
    {
        var existingRental = Create.Rental().Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);
        
        var actualResult = await _rentalService.UpdateRentalAsync(existingRental.Id, existingRental.Units, existingRental.PreparationTimeInDays);
        
        Assert.AreEqual(true, actualResult.IsSuccess);
    }
    
    [Test]
    public async Task UpdateRental_ReturnsIsSuccessTrue_WhenExistingRentalHasNotAnyBookingsAndAllParamsIsCorrect()
    {
        var existingRental = Create.Rental().Please();
        _rentalRepository.GetOrDefaultAsync(existingRental.Id).Returns(existingRental);
        
        var actualResult = await _rentalService.UpdateRentalAsync(existingRental.Id, existingRental.Units + 1, existingRental.PreparationTimeInDays);
        
        Assert.AreEqual(true, actualResult.IsSuccess);
    }

    [Test]
    public async Task UpdateRental_ReturnsIsSuccessFalse_WhenPreparationTimeMakesConflictBetweenCreatedBookings()
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
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public async Task UpdateRental_ReturnsErrorStatusConflict_WhenPreparationTimeMakesConflictBetweenCreatedBookings()
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
        
        Assert.AreEqual(UpdateRentalResultErrorStatus.Conflict, actualResult.ResultErrorStatus);
    }
    
    [Test]
    public async Task UpdateRental_ReturnsIsSuccessFalse_WhenPreparationTimeMakesConflictBetweenCurrentBookingsCountAndNewUnitsCount()
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
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public async Task UpdateRental_ReturnsErrorStatusConflict_WhenPreparationTimeMakesConflictBetweenCurrentBookingsCountAndNewUnitsCount()
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
        
        Assert.AreEqual(UpdateRentalResultErrorStatus.Conflict, actualResult.ResultErrorStatus);
    }
    
    [Test]
    public async Task UpdateRental_UpdatesRentalUnitsValueInRepository_WhenAllParamsIsCorrectAndConflictsNotFound()
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
        
        await _rentalRepository.Received(1).UpdateAsync(Arg.Any<Rental>());
    }
    
    [Test]
    public async Task UpdateRental_UpdatesRentalUnitsValueInRepositoryWithNewUnitsAndPreparationTimeInDaysValues_WhenAllParamsIsCorrectAndConflictsNotFound()
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
            .UpdateAsync(Arg.Is<Rental>(x => 
                x.Id == expectedRentalToUpdate.Id 
                && x.Units == expectedRentalToUpdate.Units
                && x.PreparationTimeInDays == expectedRentalToUpdate.PreparationTimeInDays));
    }
    
    [Test]
    public async Task UpdateRental_ReturnsIsSuccessTrue_WhenAllParamsIsCorrectAndConflictsNotFound()
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
        
        Assert.AreEqual(true, actualResult.IsSuccess);
    }
}