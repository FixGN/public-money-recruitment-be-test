using System;
using NSubstitute;
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
    private readonly DateTime _defaultDateTime = new(2022, 1, 1);
    private const int DefaultNights = 2;

    public CalendarServiceTests()
    {
        _bookingRepository = Substitute.For<IBookingRepository>();
        _rentalRepository = Substitute.For<IRentalRepository>();
        _calendarService = new CalendarService(_bookingRepository, _rentalRepository);
    }

    [Test]
    public void GetCalendarDates_ReturnsIsSuccessFalse_WhenNightsIsNegative()
    {
        var result = _calendarService.GetCalendarDates(DefaultRentalId, _defaultDateTime, -1);
        
        Assert.AreEqual(false, result.IsSuccess);
    }
    
    [Test]
    public void GetCalendarDates_ReturnsIsSuccessFalse_WhenRentalNotFound()
    {
        _rentalRepository.Get(DefaultRentalId).Returns((Rental?) null);
        
        var result = _calendarService.GetCalendarDates(DefaultRentalId, _defaultDateTime, DefaultNights);

        Assert.AreEqual(false, result.IsSuccess);
    }

    [Test]
    public void GetCalendarDates_ReturnsIsSuccessTrue_WhenNightsIsPositiveAndRentalWasFound()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).Please();
        _rentalRepository.Get(DefaultRentalId).Returns(rental);
        _bookingRepository.GetByRentalId(DefaultRentalId).Returns(Array.Empty<Booking>());
        
        var result = _calendarService.GetCalendarDates(DefaultRentalId, _defaultDateTime, DefaultNights);

        Assert.AreEqual(true, result.IsSuccess);
    }

    [Test]
    public void GetCalendarDates_ReturnsCorrectCalendarDates_WhenArgumentsAreCorrectAndBookingIsCreated()
    {
        var rental = Create.Rental().WithId(DefaultRentalId).Please();
        var booking = Create.Booking()
            .WithRentalId(DefaultRentalId)
            .WithStartDate(_defaultDateTime)
            .WithNights(DefaultNights)
            .Please();
        var bookingArray = new[] {booking};
        _rentalRepository.Get(DefaultRentalId).Returns(rental);
        _bookingRepository.GetByRentalId(DefaultRentalId).Returns(bookingArray);
        // TODO: If you a change default - tests will be broken
        var expectedResult = GetCalendarDatesResult.Success(new CalendarDate[] {
            new(_defaultDateTime, bookingArray),
            new(_defaultDateTime.AddDays(1), bookingArray)
        });

        var result = _calendarService.GetCalendarDates(DefaultRentalId, _defaultDateTime, DefaultNights);
        
        Assert.IsTrue(expectedResult.AreEqual(result));
    }
}