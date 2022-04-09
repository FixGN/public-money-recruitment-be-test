using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
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

    [Test]
    public async Task GetCalendarDates_ReturnsIsSuccessFalse_WhenNightsIsNegative()
    {
        var actualResult = await _calendarService.GetCalendarDatesAsync(DefaultRentalId, _defaultStartDate, -1);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }

    [Test]
    public async Task GetCalendarDates_ReturnsIsSuccessFalse_WhenNightsIsZero()
    {
        var actualResult = await _calendarService.GetCalendarDatesAsync(DefaultRentalId, _defaultStartDate, 0);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public async Task GetCalendarDates_ReturnsIsSuccessFalse_WhenRentalNotFound()
    {
        _rentalRepository.GetOrDefaultAsync(DefaultRentalId).ReturnsNull();
        
        var actualResult = await _calendarService.GetCalendarDatesAsync(DefaultRentalId, _defaultStartDate, DefaultNights);

        Assert.AreEqual(false, actualResult.IsSuccess);
    }

    [Test]
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

        Assert.AreEqual(true, actualResult.IsSuccess);
    }

    [Test]
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
        
        Assert.Multiple(() =>
        {
            foreach (var actualCalendarDate in actualResult.CalendarDates)
            {
                var expectedCalendarDate = expectedResult.CalendarDates.FirstOrDefault(x => x.Date == actualCalendarDate.Date);
                Assert.NotNull(expectedCalendarDate, $"Date '{actualCalendarDate.Date}' is not found in expected result");
                Assert.AreEqual(
                    expectedCalendarDate!.Bookings.Length,
                    actualCalendarDate.Bookings.Length,
                    $"Bookings count not equal in date '{expectedCalendarDate.Date}'");
                Assert.AreEqual(
                    expectedCalendarDate!.PreparationTimes.Length,
                    actualCalendarDate.PreparationTimes.Length,
                    $"PreparationTimes count not equal in date '{expectedCalendarDate.Date}'");
                    Assert.IsTrue(
                        expectedCalendarDate.Bookings.All(x => actualCalendarDate.Bookings.Any(x.AreEqual)),
                        $"Bookings values not equal in date '{expectedCalendarDate.Date}'");
                Assert.IsTrue(expectedCalendarDate.PreparationTimes
                    .All(ept => 
                        actualCalendarDate.PreparationTimes.Any(apt => ept.Unit == apt.Unit)),
                    $"PreparationTimes values not equal in date '{expectedCalendarDate.Date}'");
            }
        });
    }
}