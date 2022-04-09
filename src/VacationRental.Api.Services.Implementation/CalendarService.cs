using Microsoft.Extensions.Logging;
using VacationRental.Api.Logging.Extensions.Services;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Interfaces;
using VacationRental.Api.Services.Models.Calendar;

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

    public async Task<GetCalendarDatesResult> GetCalendarDatesAsync(
        int rentalId,
        DateTime start,
        int nights,
        CancellationToken cancellationToken = default)
    {
        _logger.GetCalendarDatesStart(rentalId, start, nights);
        cancellationToken.ThrowIfCancellationRequested();
        
        if (nights <= 0)
        {
            _logger.GetCalendarDatesNightsIsNegativeOrZero(rentalId, start, nights);
            return GetCalendarDatesResult.Fail("Nights must be positive");
        }
        
        var rental = await _rentalRepository.GetOrDefaultAsync(rentalId, cancellationToken);
        if (rental == null)
        {
            _logger.GetCalendarDatesRentalNotFound(rentalId, start, nights);
            return GetCalendarDatesResult.Fail("Rental not found");
        }

        var calendarDates = new CalendarDate[nights];
        var startDate = start.Date;
        var availableBookings = await _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                rentalId,
                startDate.AddDays(-rental.PreparationTimeInDays),
                startDate.AddDays(nights + rental.PreparationTimeInDays - 1), 
                cancellationToken);
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