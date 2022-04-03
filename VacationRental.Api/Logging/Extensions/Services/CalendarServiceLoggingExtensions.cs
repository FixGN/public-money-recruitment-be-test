using System;
using Microsoft.Extensions.Logging;

namespace VacationRental.Api.Logging.Extensions.Services;

internal static class CalendarServiceLoggingExtensions
{
    // GetCalendarDates(int rentalId, DateTime start, int nights)
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _getCalendarDatesStart;
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _getCalendarDatesNightsIsNegativeOrZero;
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _getCalendarDatesRentalNotFound;
    private static readonly Action<ILogger, int, DateTime, int, int, Exception?> _getCalendarDatesFoundBookings;
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _getCalendarDatesEnd;
    
    static CalendarServiceLoggingExtensions()
    {
        // GetCalendarDates(int rentalId, DateTime start, int nights)
        _getCalendarDatesStart = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Debug,
            new(102_000),
            "GetCalendarDates(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Start");
        _getCalendarDatesNightsIsNegativeOrZero = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Information,
            new(102_001),
            "GetCalendarDates(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Nights is negative or zero");
        _getCalendarDatesRentalNotFound = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Information,
            new(102_002),
            "GetCalendarDates(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Rental not found");
        _getCalendarDatesFoundBookings = LoggerMessage.Define<int, DateTime, int, int>(
            LogLevel.Debug,
            new(102_003),
            "GetCalendarDates(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Found {@bookingsCount} bookings");
        _getCalendarDatesEnd = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Debug,
            new(102_999),
            "CreateBooking(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - End");
    }
    
    // GetCalendarDates(int rentalId, DateTime start, int nights)
    public static void GetCalendarDatesStart(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _getCalendarDatesStart(logger, rentalId, start, nights, null);
    }
    
    public static void GetCalendarDatesNightsIsNegativeOrZero(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _getCalendarDatesNightsIsNegativeOrZero(logger, rentalId, start, nights, null);
    }
    
    public static void GetCalendarDatesRentalNotFound(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _getCalendarDatesRentalNotFound(logger, rentalId, start, nights, null);
    }
    
    public static void GetCalendarDatesFoundBookings(this ILogger logger, int rentalId, DateTime start, int nights, int bookingsCount)
    {
        _getCalendarDatesFoundBookings(logger, rentalId, start, nights, bookingsCount, null);
    }
    
    public static void GetCalendarDatesEnd(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _getCalendarDatesEnd(logger, rentalId, start, nights, null);
    }
}