using Microsoft.Extensions.Logging;

namespace VacationRental.Api.Services.Implementation.Logging.Extensions;

internal static class BookingServiceLoggingExtension
{
    // CreateBookingAsync(int rentalId, DateTime start, int nights)
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _createBookingAsyncStart;
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _createBookingAsyncNightsIsNegativeOrZero;
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _createBookingAsyncRentalNotFound;
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _createBookingAsyncAvailableUnitsNotFound;
    private static readonly Action<ILogger, int, DateTime, int, Exception?> _createBookingAsyncEnd;

    static BookingServiceLoggingExtension()
    {
        // CreateBookingAsync(int rentalId, DateTime start, int nights)
        _createBookingAsyncStart = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Debug,
            new(101_000),
            "CreateBookingAsync(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Start");
        _createBookingAsyncNightsIsNegativeOrZero = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Information,
            new(101_001),
            "CreateBookingAsync(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Nights is negative or zero");
        _createBookingAsyncRentalNotFound = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Information,
            new(101_002),
            "CreateBookingAsync(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Rental not found");
        _createBookingAsyncAvailableUnitsNotFound = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Information,
            new(101_003),
            "CreateBookingAsync(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - Available for booking units not found");
        _createBookingAsyncEnd = LoggerMessage.Define<int, DateTime, int>(
            LogLevel.Debug,
            new(101_999),
            "CreateBookingAsync(rentalId: {@rentalId}, start: {@start}, nights: {@nights}) - End");
    }
    
    // CreateBooking(int rentalId, DateTime start, int nights)
    public static void CreateBookingAsyncStart(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _createBookingAsyncStart(logger, rentalId, start, nights, null);
    }
    
    public static void CreateBookingAsyncNightsIsNegativeOrZero(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _createBookingAsyncNightsIsNegativeOrZero(logger, rentalId, start, nights, null);
    }
    
    public static void CreateBookingAsyncRentalNotFound(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _createBookingAsyncRentalNotFound(logger, rentalId, start, nights, null);
    }
    
    public static void CreateBookingAsyncAvailableUnitsNotFound(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _createBookingAsyncAvailableUnitsNotFound(logger, rentalId, start, nights, null);
    }
    
    public static void CreateBookingAsyncEnd(this ILogger logger, int rentalId, DateTime start, int nights)
    {
        _createBookingAsyncEnd(logger, rentalId, start, nights, null);
    }
}