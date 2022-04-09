using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Implementation;
using VacationRental.Api.Services.Models.Booking;
using VacationRental.Api.Tests.Unit.DSL;
using VacationRental.Api.Tests.Unit.Extensions;
using Xunit;

namespace VacationRental.Api.Tests.Unit.Services.Booking;

[Collection("Unit")]
public class GetBookingOrDefaultTests
{
    private readonly IBookingService _bookingService;
    private readonly IBookingRepository _bookingRepository;
    private readonly IRentalRepository _rentalRepository;

    private const int DefaultBookingId = 1;

    public GetBookingOrDefaultTests()
    {
        _bookingRepository = Substitute.For<IBookingRepository>();
        _rentalRepository = Substitute.For<IRentalRepository>();
        _bookingService = new BookingService(_bookingRepository, _rentalRepository, new NullLogger<BookingService>());
    }
    
    [Fact]
    public async Task GivenCreatedBooking_WhenBookingExists_ThenReturnsBooking()
    {
        var expectedBooking = Create.Booking().WithId(DefaultBookingId).Please();
        _bookingRepository.GetOrDefaultAsync(DefaultBookingId).Returns(expectedBooking);

        var actualBooking = await _bookingService.GetBookingOrDefaultAsync(DefaultBookingId);
        
        Assert.NotNull(actualBooking);
    }
    
    [Fact]
    public async Task GivenCreatedBooking_WhenBookingExists_ThenReturnsCorrectBooking()
    {
        var expectedBooking = Create.Booking().WithId(DefaultBookingId).Please();
        _bookingRepository.GetOrDefaultAsync(DefaultBookingId).Returns(expectedBooking);

        var actualBooking = await _bookingService.GetBookingOrDefaultAsync(DefaultBookingId);
        
        Assert.True(actualBooking!.AreEqual(expectedBooking), "Bookings are not equal");
    }
    
    [Fact]
    public async Task GivenNoBookings_WhenBookingNotExists_ThenReturnsNull()
    {
        _bookingRepository.GetOrDefaultAsync(DefaultBookingId).ReturnsNull();

        var actualBooking = await _bookingService.GetBookingOrDefaultAsync(DefaultBookingId);
        
        Assert.Null(actualBooking);
    }
}