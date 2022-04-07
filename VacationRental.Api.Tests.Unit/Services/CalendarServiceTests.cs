using System;
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
    public void GetCalendarDates_ReturnsIsSuccessFalse_WhenNightsIsNegative()
    {
        var actualResult = _calendarService.GetCalendarDates(DefaultRentalId, _defaultStartDate, -1);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }

    [Test]
    public void GetCalendarDates_ReturnsIsSuccessFalse_WhenNightsIsZero()
    {
        var actualResult = _calendarService.GetCalendarDates(DefaultRentalId, _defaultStartDate, 0);
        
        Assert.AreEqual(false, actualResult.IsSuccess);
    }
    
    [Test]
    public void GetCalendarDates_ReturnsIsSuccessFalse_WhenRentalNotFound()
    {
        _rentalRepository.GetOrDefault(DefaultRentalId).ReturnsNull();
        
        var actualResult = _calendarService.GetCalendarDates(DefaultRentalId, _defaultStartDate, DefaultNights);

        Assert.AreEqual(false, actualResult.IsSuccess);
    }

    [Test]
    public void GetCalendarDates_ReturnsIsSuccessTrue_WhenNightsIsPositiveAndRentalWasFound()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).Please();
        _rentalRepository.GetOrDefault(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                defaultStartDate,
                defaultStartDate.AddDays(DefaultNights - 1))
            .Returns(Array.Empty<Booking>());        
        var actualResult = _calendarService.GetCalendarDates(DefaultRentalId, _defaultStartDate, DefaultNights);

        Assert.AreEqual(true, actualResult.IsSuccess);
    }

    [Test]
    public void GetCalendarDates_ReturnsCorrectCalendarDates_WhenArgumentsAreCorrectAndBookingIsCreated()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).Please();
        var booking = Create.Booking()
            .WithRentalId(DefaultRentalId)
            .WithStartDate(_defaultStartDate)
            .WithNights(DefaultNights)
            .Please();
        var bookingArray = new[] {booking};
        _rentalRepository.GetOrDefault(DefaultRentalId).Returns(rental);
        var defaultStartDate = _defaultStartDate.Date;
        _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                DefaultRentalId,
                defaultStartDate,
                defaultStartDate.AddDays(DefaultNights - 1))
            .Returns(Array.Empty<Booking>());
        // TODO: Fix test after implementation of first task
        var expectedResult = GetCalendarDatesResult.Success(new CalendarDate[] {
            new(_defaultStartDate, bookingArray, Array.Empty<CalendarPreparationTime>()),
            new(_defaultStartDate.AddDays(1), bookingArray, Array.Empty<CalendarPreparationTime>())
        });

        var actualResult = _calendarService.GetCalendarDates(DefaultRentalId, _defaultStartDate, DefaultNights);
        
        Assert.IsTrue(expectedResult.AreEqual(actualResult));
    }
}