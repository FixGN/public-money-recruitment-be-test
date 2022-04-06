using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using VacationRental.Api.Logging.Extensions.Services;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Models;

namespace VacationRental.Api.Services.Implementation;

public class CalendarService : ICalendarService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly ILogger<CalendarService> _logger;

    public CalendarService(IBookingRepository bookingRepository, IRentalRepository rentalRepository, ILogger<CalendarService> logger)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public GetCalendarDatesResult GetCalendarDates(int rentalId, DateTime start, int nights)
    {
        _logger.GetCalendarDatesStart(rentalId, start, nights);
        if (nights <= 0)
        {
            _logger.GetCalendarDatesNightsIsNegativeOrZero(rentalId, start, nights);
            return GetCalendarDatesResult.Fail("Nights must be positive");
        }
        // TODO: It's a part of contract? Can I delete this? 
        var rental = _rentalRepository.GetOrDefault(rentalId);
        if (rental == null)
        {
            _logger.GetCalendarDatesRentalNotFound(rentalId, start, nights);
            return GetCalendarDatesResult.Fail("Rental not found");
        }

        var calendarDates = new CalendarDate[nights];
        var startDate = start.Date;
        // TODO: Make test for preparationTimeInDays period (show preparationTime after booking, if unit was booked, but show days after booking)
        var availableBookings = _bookingRepository
            .GetByRentalIdAndDatePeriod(
                rentalId,
                startDate.AddDays(-rental.PreparationTimeInDays),
                startDate.AddDays(nights + rental.PreparationTimeInDays - 1));
        _logger.GetCalendarDatesFoundBookings(rentalId, start, nights, availableBookings.Length);
        
        for (var i = 0; i < nights; i++)
        {
            var date = start.Date.AddDays(i);
            var bookings = availableBookings
                .Where(x => x.Start <= date && x.Start.AddDays(x.Nights) > date)
                .ToArray();
            var preparationTime = availableBookings
                .Where(x => x.Start.AddDays(x.Nights) <= date
                            && x.Start.AddDays(x.Nights + rental.PreparationTimeInDays) > date)
                .Select(x => new CalendarPreparationTime(x.Unit))
                .ToArray();

            calendarDates[i] = new CalendarDate(date, bookings, preparationTime);
        }

        _logger.GetCalendarDatesEnd(rentalId, start, nights);
        return GetCalendarDatesResult.Success(calendarDates);
    }
}