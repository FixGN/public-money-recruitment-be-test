using Microsoft.Extensions.Logging;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Implementation.Logging.Extensions;
using VacationRental.Api.Services.Interfaces;
using VacationRental.Api.Services.Models.Calendar;

namespace VacationRental.Api.Services.Implementation;

public class CalendarService : ICalendarService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<CalendarService> _logger;
    private readonly IRentalRepository _rentalRepository;

    public CalendarService(IBookingRepository bookingRepository, IRentalRepository rentalRepository, ILogger<CalendarService> logger)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<GetCalendarDatesResult> GetCalendarDatesAsync(int rentalId,
        DateOnly startDate,
        int nights,
        CancellationToken cancellationToken = default)
    {
        _logger.GetCalendarDatesAsyncStart(rentalId, startDate, nights);
        cancellationToken.ThrowIfCancellationRequested();

        if (nights <= 0)
        {
            _logger.GetCalendarDatesAsyncNightsIsNegativeOrZero(rentalId, startDate, nights);
            return GetCalendarDatesResult.Fail("Nights must be positive");
        }

        var rental = await _rentalRepository.GetOrDefaultAsync(rentalId, cancellationToken);
        if (rental == null)
        {
            _logger.GetCalendarDatesAsyncRentalNotFound(rentalId, startDate, nights);
            return GetCalendarDatesResult.Fail("Rental not found");
        }

        var calendarDates = new CalendarDate[nights];
        var availableBookings = await _bookingRepository
            .GetByRentalIdAndDatePeriodAsync(
                rentalId,
                startDate.AddDays(-rental.PreparationTimeInDays),
                startDate.AddDays(nights + rental.PreparationTimeInDays - 1),
                cancellationToken);
        _logger.GetCalendarDatesAsyncFoundBookings(rentalId, startDate, nights, availableBookings.Length);

        for (var i = 0; i < nights; i++)
        {
            var date = startDate.AddDays(i);
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

        _logger.GetCalendarDatesAsyncEnd(rentalId, startDate, nights);
        return GetCalendarDatesResult.Success(calendarDates);
    }
}