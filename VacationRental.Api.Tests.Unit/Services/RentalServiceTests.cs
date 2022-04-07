using System;
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
    public void GetRental_ReturnsNull_WhenRentalNotExists()
    {
        _rentalRepository.GetOrDefault(DefaultRentalId).ReturnsNull();

        var actualRental = _rentalService.GetRentalOrDefault(DefaultRentalId);
        
        Assert.IsNull(actualRental);
    }
    
    [Test]
    public void GetRental_ReturnsRental_WhenRentalExists()
    {
        var rental = Create.Rental().Please();
        _rentalRepository.GetOrDefault(DefaultRentalId).Returns(rental);

        var actualRental = _rentalService.GetRentalOrDefault(DefaultRentalId);
        
        Assert.IsInstanceOf<Rental>(actualRental);
    }
    
    [Test]
    public void GetRental_ReturnsCorrectRental_WhenRentalExists()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.GetOrDefault(DefaultRentalId).Returns(expectedRental);

        var actualRental = _rentalService.GetRentalOrDefault(DefaultRentalId);
        
        Assert.IsTrue(actualRental!.AreEqual(expectedRental));
    }

    [Test]
    public void CreateRental_CreatesRental()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.Create(expectedRental.Units, expectedRental.PreparationTimeInDays).Returns(expectedRental);

        _rentalService.CreateRental(expectedRental.Units, expectedRental.PreparationTimeInDays);

        _rentalRepository.Received(1).Create(expectedRental.Units, expectedRental.PreparationTimeInDays);
    }
    
    [Test]
    public void CreateRental_ReturnsCreatedRental()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.Create(expectedRental.Units, expectedRental.PreparationTimeInDays).Returns(expectedRental);

        var actualRental = _rentalService.CreateRental(expectedRental.Units, expectedRental.PreparationTimeInDays);
        
        Assert.IsTrue(actualRental.AreEqual(expectedRental));
    }
    
    [Test]
    public void UpdateRental_ReturnsIsSuccessFalse_WhenUnitsIsNegativeNumber()
    {
        var actualResult = _rentalService.UpdateRental(DefaultRentalId, -1, DefaultPreparationTimeInDays);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public void UpdateRental_ReturnsErrorStatusValidationFailed_WhenUnitsIsNegativeNumber()
    {
        var actualResult = _rentalService.UpdateRental(DefaultRentalId, -1, DefaultPreparationTimeInDays);
        
        Assert.AreEqual(UpdateRentalErrorStatus.ValidationFailed, actualResult.ErrorStatus);
    }
    
    [Test]
    public void UpdateRental_ReturnsIsSuccessFalse_WhenPreparationTimeInDaysIsNegativeNumber()
    {
        var actualResult = _rentalService.UpdateRental(DefaultRentalId, DefaultUnits, -1);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public void UpdateRental_ReturnsErrorStatusValidationFailed_WhenPreparationTimeInDaysIsNegativeNumber()
    {
        var actualResult = _rentalService.UpdateRental(DefaultRentalId, DefaultUnits, -1);
        
        Assert.AreEqual(UpdateRentalErrorStatus.ValidationFailed, actualResult.ErrorStatus);
    }
    
    [Test]
    public void UpdateRental_ReturnsIsSuccessFalse_WhenRentalNotExists()
    {
        _rentalRepository.GetOrDefault(DefaultRentalId).ReturnsNull();
        
        var actualResult = _rentalService.UpdateRental(DefaultRentalId, DefaultUnits, DefaultPreparationTimeInDays);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public void UpdateRental_ReturnsErrorStatusRentalNotFound_WhenRentalNotExists()
    {
        _rentalRepository.GetOrDefault(DefaultRentalId).ReturnsNull();
        
        var actualResult = _rentalService.UpdateRental(DefaultRentalId, DefaultUnits, DefaultPreparationTimeInDays);
        
        Assert.AreEqual(UpdateRentalErrorStatus.RentalNotFound, actualResult.ErrorStatus);
    }

    [Test]
    public void UpdateRental_ReturnsIsSuccessTrue_WhenExistingRentalHasSameUnitsAndPreparationTimeInDaysValues()
    {
        var existingRental = Create.Rental().Please();
        _rentalRepository.GetOrDefault(existingRental.Id).Returns(existingRental);
        
        var actualResult = _rentalService.UpdateRental(existingRental.Id, existingRental.Units, existingRental.PreparationTimeInDays);
        
        Assert.AreEqual(true, actualResult.IsSuccess);
    }
    
    [Test]
    public void UpdateRental_ReturnsIsSuccessTrue_WhenExistingRentalHasNotAnyBookingsAndAllParamsIsCorrect()
    {
        var existingRental = Create.Rental().Please();
        _rentalRepository.GetOrDefault(existingRental.Id).Returns(existingRental);
        
        var actualResult = _rentalService.UpdateRental(existingRental.Id, existingRental.Units + 1, existingRental.PreparationTimeInDays);
        
        Assert.AreEqual(true, actualResult.IsSuccess);
    }

    [Test]
    public void UpdateRental_ReturnsIsSuccessFalse_WhenPreparationTimeMakesConflictBetweenCreatedBookings()
    {
        var existingRental = Create.Rental().WithUnits(1).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefault(existingRental.Id).Returns(existingRental);
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
        _bookingRepository.GetByRentalId(existingRental.Id).Returns(existingBookings);

        var actualResult = _rentalService.UpdateRental(
            existingRental.Id,
            existingRental.Units, 
            existingRental.PreparationTimeInDays + 1);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public void UpdateRental_ReturnsErrorStatusConflict_WhenPreparationTimeMakesConflictBetweenCreatedBookings()
    {
        var existingRental = Create.Rental().WithUnits(1).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefault(existingRental.Id).Returns(existingRental);
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
        _bookingRepository.GetByRentalId(existingRental.Id).Returns(existingBookings);

        var actualResult = _rentalService.UpdateRental(
            existingRental.Id,
            existingRental.Units,
            existingRental.PreparationTimeInDays + 1);
        
        Assert.AreEqual(UpdateRentalErrorStatus.Conflict, actualResult.ErrorStatus);
    }
    
    [Test]
    public void UpdateRental_ReturnsIsSuccessFalse_WhenPreparationTimeMakesConflictBetweenCurrentBookingsCountAndNewUnitsCount()
    {
        var existingRental = Create.Rental().WithUnits(3).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefault(existingRental.Id).Returns(existingRental);
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
        _bookingRepository.GetByRentalId(existingRental.Id).Returns(existingBookings);

        var actualResult = _rentalService.UpdateRental(
            existingRental.Id,
            existingRental.Units - 1,
            existingRental.PreparationTimeInDays);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public void UpdateRental_ReturnsErrorStatusConflict_WhenPreparationTimeMakesConflictBetweenCurrentBookingsCountAndNewUnitsCount()
    {
        var existingRental = Create.Rental().WithUnits(3).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefault(existingRental.Id).Returns(existingRental);
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
        _bookingRepository.GetByRentalId(existingRental.Id).Returns(existingBookings);

        var actualResult = _rentalService.UpdateRental(
            existingRental.Id,
            existingRental.Units - 1,
            existingRental.PreparationTimeInDays);
        
        Assert.AreEqual(UpdateRentalErrorStatus.Conflict, actualResult.ErrorStatus);
    }
    
    [Test]
    public void UpdateRental_UpdatesRentalUnitsValueInRepository_WhenAllParamsIsCorrectAndConflictsNotFound()
    {
        var existingRental = Create.Rental().WithUnits(3).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefault(existingRental.Id).Returns(existingRental);
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
        _bookingRepository.GetByRentalId(existingRental.Id).Returns(existingBookings);

        _rentalService.UpdateRental(
            existingRental.Id,
            existingRental.Units - 1,
            existingRental.PreparationTimeInDays + 1);
        
        _rentalRepository.Received(1).Update(Arg.Any<Rental>());
    }
    
    [Test]
    public void UpdateRental_UpdatesRentalUnitsValueInRepositoryWithNewUnitsAndPreparationTimeInDaysValues_WhenAllParamsIsCorrectAndConflictsNotFound()
    {
        var existingRental = Create.Rental().WithUnits(3).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefault(existingRental.Id).Returns(existingRental);
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
        _bookingRepository.GetByRentalId(existingRental.Id).Returns(existingBookings);
        var expectedRentalToUpdate = Create.Rental()
            .WithId(existingRental.Id)
            .WithUnits(existingRental.Units - 1)
            .WithPreparationTimeInDays(existingRental.PreparationTimeInDays + 1)
            .Please();

        _rentalService.UpdateRental(
            existingRental.Id,
            expectedRentalToUpdate.Units,
            expectedRentalToUpdate.PreparationTimeInDays);
        
        _rentalRepository
            .Received(1)
            .Update(Arg.Is<Rental>(x => 
                x.Id == expectedRentalToUpdate.Id 
                && x.Units == expectedRentalToUpdate.Units
                && x.PreparationTimeInDays == expectedRentalToUpdate.PreparationTimeInDays));
    }
    
    [Test]
    public void UpdateRental_ReturnsIsSuccessTrue_WhenAllParamsIsCorrectAndConflictsNotFound()
    {
        var existingRental = Create.Rental().WithUnits(3).WithPreparationTimeInDays(2).Please();
        _rentalRepository.GetOrDefault(existingRental.Id).Returns(existingRental);
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
        _bookingRepository.GetByRentalId(existingRental.Id).Returns(existingBookings);

        var actualResult = _rentalService.UpdateRental(
            existingRental.Id,
            existingRental.Units - 1,
            existingRental.PreparationTimeInDays + 1);
        
        Assert.AreEqual(true, actualResult.IsSuccess);
    }
}