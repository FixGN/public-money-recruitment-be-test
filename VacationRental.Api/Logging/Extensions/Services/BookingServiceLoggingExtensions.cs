using System;
using Microsoft.Extensions.Logging;

namespace VacationRental.Api.Logging.Extensions.Services;

internal static class BookingServiceLoggingExtension
{
    // CreateBooking(int rentalId, DateTime start, int nights)
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _createBookingStart;
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _createBookingNightsIsNegativeOrZero;
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _createBookingRentalNotFound;
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _createBookingAvailableUnitsNotFound;
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _createBookingEnd;

    static BookingServiceLoggingExtension()
    {
        // CreateBooking(int rentalId, DateTime start, int nights)
        _createBookingStart = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Debug,
            new(101_000),
            "CreateBooking(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Start");
        _createBookingNightsIsNegativeOrZero = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Debug,
            new(101_000),
            "CreateBooking(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Nights is negative or zero");
        _createBookingRentalNotFound = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Debug,
            new(101_000),
            "CreateBooking(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Rental not found");
        _createBookingAvailableUnitsNotFound = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Debug,
            new(101_000),
            "CreateBooking(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Available for booking units not found");
        _createBookingEnd = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Debug,
            new(101_999),
            "CreateBooking(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - End");
    }
    
    // CreateBooking(int rentalId, DateTime start, int nights)
    public static void CreateBookingStart(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _createBookingStart(logger, rentalId, start, nights, null);
    }
    
    public static void CreateBookingNightsIsNegativeOrZero(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _createBookingNightsIsNegativeOrZero(logger, rentalId, start, nights, null);
    }
    
    public static void CreateBookingRentalNotFound(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _createBookingRentalNotFound(logger, rentalId, start, nights, null);
    }
    
    public static void CreateBookingAvailableUnitsNotFound(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _createBookingAvailableUnitsNotFound(logger, rentalId, start, nights, null);
    }
    
    public static void CreateBookingEnd(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _createBookingEnd(logger, rentalId, start, nights, null);
    }
}