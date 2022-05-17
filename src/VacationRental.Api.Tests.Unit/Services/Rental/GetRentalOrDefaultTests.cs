using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Implementation;
using VacationRental.Api.Services.Interfaces;
using VacationRental.Api.Tests.Unit.DSL;
using VacationRental.Api.Tests.Unit.Extensions;
using Xunit;

namespace VacationRental.Api.Tests.Unit.Services.Rental;

[Collection("Unit")]
public class GetRentalOrDefaultTests
{
    private const int DefaultRentalId = 1;

    private readonly IRentalRepository _rentalRepository;
    private readonly IRentalService _rentalService;

    public GetRentalOrDefaultTests()
    {
        _rentalRepository = Substitute.For<IRentalRepository>();
        var postBookingRepository = Substitute.For<IBookingRepository>();
        _rentalService = new RentalService(_rentalRepository, postBookingRepository, new NullLogger<RentalService>());
    }

    [Fact]
    public async Task GivenNoRental_WhenRentalNotExists_ThenReturnsNull()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();

        var actualRental = await _rentalService.GetRentalOrDefaultAsync(DefaultRentalId);

        Assert.Null(actualRental);
    }

    [Fact]
    public async Task GivenRental_WhenRentalExists_ThenReturnsRental()
    {
        var rental = Create.Rental().Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);

        var actualRental = await _rentalService.GetRentalOrDefaultAsync(DefaultRentalId);

        Assert.IsType<Models.Rental>(actualRental);
    }

    [Fact]
    public async Task GivenRental_WhenRentalExists_ThenReturnsCorrectRental()
    {
        var expectedRental = Create.Rental().Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(expectedRental);

        var actualRental = await _rentalService.GetRentalOrDefaultAsync(DefaultRentalId);

        Assert.True(actualRental!.AreEqual(expectedRental));
    }
}