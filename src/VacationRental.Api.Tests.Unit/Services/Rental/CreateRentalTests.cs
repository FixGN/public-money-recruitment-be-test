using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Implementation;
using VacationRental.Api.Services.Interfaces;
using VacationRental.Api.Services.Models.Rental;
using VacationRental.Api.Tests.Unit.DSL;
using VacationRental.Api.Tests.Unit.Extensions;
using Xunit;

namespace VacationRental.Api.Tests.Unit.Services.Rental;

[Collection("Unit")]
public class CreateRentalTests
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IRentalService _rentalService;

    private const int DefaultUnits = 1;
    private const int DefaultPreparationTimeInDays = 1;

    public CreateRentalTests()
    {
        _rentalRepository = Substitute.For<IRentalRepository>();
        var postBookingRepository = Substitute.For<IBookingRepository>();
        _rentalService = new RentalService(_rentalRepository, postBookingRepository, new NullLogger<RentalService>());
    }

    [Fact]
    public async Task GivenNoRental_WhenUnitsIsNegativeNumber_ThenReturnsIsSuccessFalse()
    {
        var actualRentalResult = await _rentalService.CreateRentalAsync(-1, DefaultPreparationTimeInDays);
        
        Assert.False(actualRentalResult.IsSuccess);
    }

    [Fact]
    public async Task GivenNoRental_WhenUnitsIsNegativeNumber_ThenReturnsErrorStatusValidationFail()
    {
        var actualRentalResult = await _rentalService.CreateRentalAsync(-1, DefaultPreparationTimeInDays);
        
        Assert.Equal(CreateRentalResultErrorStatus.ValidationFailed, actualRentalResult.ErrorStatus);
    }
    
    [Fact]
    public async Task GivenNoRental_WhenPreparationTimeInDaysIsNegativeNumber_ThenReturnsIsSuccessFalse()
    {
        var actualRentalResult = await _rentalService.CreateRentalAsync(DefaultUnits, -1);
        
        Assert.False(actualRentalResult.IsSuccess);
    }

    [Fact]
    public async Task GivenNoRental_WhenPreparationTimeInDaysIsNegativeNumber_ThenReturnsErrorStatusValidationFail()
    {
        var actualRentalResult = await _rentalService.CreateRentalAsync(DefaultUnits, -1);
        
        Assert.Equal(CreateRentalResultErrorStatus.ValidationFailed, actualRentalResult.ErrorStatus);
    }
    
    [Fact]
    public async Task GivenNoRental_WhenAllParametersIsCorrect_ThenReturnsIsSuccessTrue()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.CreateAsync(expectedRental.Units, expectedRental.PreparationTimeInDays).Returns(expectedRental);

        var actualRentalResult = await _rentalService.CreateRentalAsync(expectedRental.Units, expectedRental.PreparationTimeInDays);
        
        Assert.True(actualRentalResult.IsSuccess);
    }
    
    [Fact]
    public async Task GivenNoRental_WhenAllParametersIsCorrect_ThenReturnsCorrectRental()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.CreateAsync(expectedRental.Units, expectedRental.PreparationTimeInDays).Returns(expectedRental);

        var actualRentalResult = await _rentalService.CreateRentalAsync(expectedRental.Units, expectedRental.PreparationTimeInDays);
        
        Assert.True(actualRentalResult.Rental!.AreEqual(expectedRental));
    }
}