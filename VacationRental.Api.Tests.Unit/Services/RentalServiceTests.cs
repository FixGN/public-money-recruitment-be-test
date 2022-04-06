using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Implementation;
using VacationRental.Api.Tests.Unit.DSL;
using VacationRental.Api.Tests.Unit.Extensions;

namespace VacationRental.Api.Tests.Unit.Services;

public class RentalServiceTests
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IRentalService _rentalService;
    private readonly IBookingRepository _bookingRepository;

    private const int DefaultRentalId = 1;

    public RentalServiceTests()
    {
        _rentalRepository = Substitute.For<IRentalRepository>();
        _bookingRepository = Substitute.For<IBookingRepository>();
        _rentalService = new RentalService(_rentalRepository, _bookingRepository);
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
    public void CreateRental_CreateRental()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.Create(expectedRental.Units, expectedRental.PreparationTimeInDays).Returns(expectedRental);

        _rentalService.CreateRental(expectedRental.Units, expectedRental.PreparationTimeInDays);

        _rentalRepository.Received(1).Create(expectedRental.Units, expectedRental.PreparationTimeInDays);
    }
    
    [Test]
    public void CreateRental_ReturnCreatedRental()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.Create(expectedRental.Units, expectedRental.PreparationTimeInDays).Returns(expectedRental);

        var actualRental = _rentalService.CreateRental(expectedRental.Units, expectedRental.PreparationTimeInDays);
        
        Assert.IsTrue(actualRental.AreEqual(expectedRental));
    }
}