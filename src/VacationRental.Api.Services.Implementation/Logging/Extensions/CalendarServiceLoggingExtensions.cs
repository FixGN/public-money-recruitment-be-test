using Microsoft.Extensions.Logging;

namespace VacationRental.Api.Services.Implementation.Logging.Extensions;

internal static class CalendarServiceLoggingExtensions
{
    // GetCalendarDatesAsync(int rentalId, DateOnly start, int nights)
    private static readonly Action<ILogger, int, DateOnly, int, Exception?> _getCalendarDatesAsyncStart;
    private static readonly Action<ILogger, int, DateOnly, int, Exception?> _getCalendarDatesAsyncNightsIsNegativeOrZero;
    private static readonly Action<ILogger, int, DateOnly, int, Exception?> _getCalendarDatesAsyncRentalNotFound;
    private static readonly Action<ILogger, int, DateOnly, int, int, Exception?> _getCalendarDatesAsyncFoundBookings;
    private static readonly Action<ILogger, int, DateOnly, int, Exception?> _getCalendarDatesAsyncEnd;

    static CalendarServiceLoggingExtensions()
    {
        // GetCalendarDatesAsync(int rentalId, DateOnly start, int nights)
        _getCalendarDatesAsyncStart = LoggerMessage.Define<int, DateOnly, int>(
            LogLevel.Debug,
            new EventId(102_000),
            "GetCalendarDatesAsync(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Start");
        _getCalendarDatesAsyncNightsIsNegativeOrZero = LoggerMessage.Define<int, DateOnly, int>(
            LogLevel.Information,
            new EventId(102_001),
            "GetCalendarDatesAsync(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Nights is negative or zero");
        _getCalendarDatesAsyncRentalNotFound = LoggerMessage.Define<int, DateOnly, int>(
            LogLevel.Information,
            new EventId(102_002),
            "GetCalendarDatesAsync(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Rental not found");
        _getCalendarDatesAsyncFoundBookings = LoggerMessage.Define<int, DateOnly, int, int>(
            LogLevel.Debug,
            new EventId(102_003),
            "GetCalendarDatesAsync(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Found {@bookingsCount} bookings");
        _getCalendarDatesAsyncEnd = LoggerMessage.Define<int, DateOnly, int>(
            LogLevel.Debug,
            new EventId(102_999),
            "GetCalendarDatesAsync(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - End");
    }

    // GetCalendarDatesAsync(int rentalId, DateOnly start, int nights)
    public static void GetCalendarDatesAsyncStart(this ILogger logger, int rentalId, DateOnly start, int nights)
    {
        _getCalendarDatesAsyncStart(logger, rentalId, start, nights, null);
    }

    public static void GetCalendarDatesAsyncNightsIsNegativeOrZero(this ILogger logger, int rentalId, DateOnly start, int nights)
    {
        _getCalendarDatesAsyncNightsIsNegativeOrZero(logger, rentalId, start, nights, null);
    }

    public static void GetCalendarDatesAsyncRentalNotFound(this ILogger logger, int rentalId, DateOnly start, int nights)
    {
        _getCalendarDatesAsyncRentalNotFound(logger, rentalId, start, nights, null);
    }

    public static void GetCalendarDatesAsyncFoundBookings(this ILogger logger, int rentalId, DateOnly start, int nights, int bookingsCount)
    {
        _getCalendarDatesAsyncFoundBookings(logger, rentalId, start, nights, bookingsCount, null);
    }

    public static void GetCalendarDatesAsyncEnd(this ILogger logger, int rentalId, DateOnly start, int nights)
    {
        _getCalendarDatesAsyncEnd(logger, rentalId, start, nights, null);
    }
}