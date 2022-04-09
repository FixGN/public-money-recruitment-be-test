using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Implementation;
using VacationRental.Api.Services.Models.Calendar;
using VacationRental.Api.Tests.Unit.DSL;
using VacationRental.Api.Tests.Unit.Extensions;
using Xunit;

namespace VacationRental.Api.Tests.Unit.Services;

[Collection("Unit")]
public class CalendarServiceTests
{
    private readonly ICalendarService _calendarService;
    private readonly IRentalRepository _rentalRepository;
    private readonly IBookingRepository _bookingRepository;

    private const int DefaultRentalId = 1;
    private readonly DateTime _defaultStartDate = new(2022, 1, 1);
    private const int DefaultNights = 2;

    public CalendarServiceTests()
    {
        _bookingRepository = Substitute.For<IBookingRepository>();
        _rentalRepository = Substitute.For<IRentalRepository>();
        _calendarService = new CalendarService(_bookingRepository, _rentalRepository, new NullLogger<CalendarService>());
    }

    [Fact]
    public async Task GetCalendarDates_ReturnsIsSuccessFalse_WhenNightsIsNegative()
    {
        var actualResult = await _calendarService.GetCalendarDatesAsync(DefaultRentalId, _defaultStartDate, -1);
        
        Assert.False(actualResult.IsSuccess);
    }

    [Fact]
    public async Task GetCalendarDates_ReturnsIsSuccessFalse_WhenNightsIsZero()
    {
        var actualResult = await _calendarService.GetCalendarDatesAsync(DefaultRentalId, _defaultStartDate, 0);
        
        Assert.False(actualResult.IsSuccess);
    }
    
    [Fact]
    public async Task GetCalendarDates_ReturnsIsSuccessFalse_WhenRentalNotFound()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();
        
        var actualResult = await _calendarService.GetCalendarDatesAsync(DefaultRentalId, _defaultStartDate, DefaultNights);

        Assert.False(actualResult.IsSuccess);
    }

    [Fact]
    public async Task GetCalendarDates_ReturnsIsSuccessTrue_WhenNightsIsPositiveAndRentalWasFound()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                defaultStartDate,
                defaultStartDate.AddDays(DefaultNights - 1))
            .Returns(Array.Empty<Booking>());        
        var actualResult = await _calendarService.GetCalendarDatesAsync(DefaultRentalId, _defaultStartDate, DefaultNights);

        Assert.True(actualResult.IsSuccess);
    }

    [Fact]
    public async Task GetCalendarDates_ReturnsCorrectCalendarDates_WhenArgumentsAreCorrectAndBookingIsCreated()
    {
        const int getCalendarNightsCount = 4;
        var rental = Create.Rental().WithId(DefaultRentalId).WithPreparationTimeInDays(1).Please();
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).Returns(rental);

        var booking = Create.Booking()
            .WithRentalId(DefaultRentalId)
            .WithStartDate(_defaultStartDate)
            .WithNights(2)
            .Please();
        var bookingArray = new[] {booking};
        _bookingRepository.GetByRentalIdAndDatePeriodAsync(
            rental.Id,
            booking.Start.AddDays(-rental.PreparationTimeInDays),
            booking.Start.AddDays(getCalendarNightsCount + rental.PreparationTimeInDays - 1),
            Arg.Any<CancellationToken>())
            .Returns(bookingArray);
        
        var expectedResult = GetCalendarDatesResult.Success(new CalendarDate[] {
            new(_defaultStartDate, bookingArray, Array.Empty<CalendarPreparationTime>()),
            new(_defaultStartDate.AddDays(1), bookingArray, Array.Empty<CalendarPreparationTime>()),
            new(_defaultStartDate.AddDays(2), Array.Empty<Booking>(), new[] {new CalendarPreparationTime(booking.Unit)}),
            new(_defaultStartDate.AddDays(3), Array.Empty<Booking>(), Array.Empty<CalendarPreparationTime>())
        });

        var actualResult = await _calendarService.GetCalendarDatesAsync(DefaultRentalId, _defaultStartDate, getCalendarNightsCount);
        
        // Assert
        // Due to I can't create message for Assert and I can't do multiply asserts, NUnit looks better
        foreach (var actualCalendarDate in actualResult.CalendarDates)
        {
            var expectedCalendarDate = expectedResult.CalendarDates.FirstOrDefault(x => x.Date == actualCalendarDate.Date);
            Assert.NotNull(expectedCalendarDate);
            Assert.Equal(expectedCalendarDate!.Bookings.Length, actualCalendarDate.Bookings.Length);
            Assert.Equal(expectedCalendarDate!.PreparationTimes.Length, actualCalendarDate.PreparationTimes.Length);
            Assert.True(expectedCalendarDate.Bookings.All(x => actualCalendarDate.Bookings.Any(x.AreEqual)));
            Assert.True(expectedCalendarDate.PreparationTimes.All(ept 
                    => actualCalendarDate.PreparationTimes.Any(apt => ept.Unit == apt.Unit)));
        }
    }
}