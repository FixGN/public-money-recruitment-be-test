using Microsoft.Extensions.Logging;

namespace VacationRental.Api.Services.Implementation.Logging.Extensions;

internal static class RentalServiceLoggingExtension
{
    // UpdateRentalAsync(int id, int units, int preparationTimeInDays)
    private static readonly Action<ILogger, int, int, int, Exception?> _updateRentalAsyncStart;
    private static readonly Action<ILogger, int, int, int, Exception?> _updateRentalAsyncUnitsIsNegative;
    private static readonly Action<ILogger, int, int, int, Exception?> _updateRentalAsyncPreparationTimeInDaysIsNegative;
    private static readonly Action<ILogger, int, int, int, Exception?> _updateRentalAsyncRentalNotFound;
    private static readonly Action<ILogger, int, int, int, Exception?> _updateRentalAsyncNothingToChange;
    private static readonly Action<ILogger, int, int, int, DateTime, Exception?> _updateRentalAsyncNumberOfUnitsConflict;
    private static readonly Action<ILogger, int, int, int, string, Exception?> _updateRentalAsyncDbConcurrencyExceptionWasThrown;
    private static readonly Action<ILogger, int, int, int, DateTime, int, Exception?>
        _updateRentalAsyncNumberOfPreparationTimeInDaysConflict;

    private static readonly Action<ILogger, int, int, int, Exception?> _updateRentalAsyncEnd;

    static RentalServiceLoggingExtension()
    {
        // UpdateRentalAsync(int id, int units, int preparationTimeInDays)
        _updateRentalAsyncStart = LoggerMessage.Define<int, int, int>(
            LogLevel.Debug,
            new EventId(103_000),
            "UpdateRentalAsync(id: {@id}, units: {@units}, preparationTimeInDays: {@preparationTimeInDays}) - Start");
        _updateRentalAsyncUnitsIsNegative = LoggerMessage.Define<int, int, int>(
            LogLevel.Information,
            new EventId(103_001),
            "UpdateRentalAsync(id: {@id}, units: {@units}, preparationTimeInDays: {@preparationTimeInDays}) - " +
            "Units count is negative");
        _updateRentalAsyncPreparationTimeInDaysIsNegative = LoggerMessage.Define<int, int, int>(
            LogLevel.Information,
            new EventId(103_002),
            "UpdateRentalAsync(id: {@id}, units: {@units}, preparationTimeInDays: {@preparationTimeInDays}) - " +
            "PreparationTimeInDays is negative");
        _updateRentalAsyncRentalNotFound = LoggerMessage.Define<int, int, int>(
            LogLevel.Information,
            new EventId(103_003),
            "UpdateRentalAsync(id: {@id}, units: {@units}, preparationTimeInDays: {@preparationTimeInDays}) - " +
            "Rental not found");
        _updateRentalAsyncNothingToChange = LoggerMessage.Define<int, int, int>(
            LogLevel.Debug,
            new EventId(103_004),
            "UpdateRentalAsync(id: {@id}, units: {@units}, preparationTimeInDays: {@preparationTimeInDays}) - " +
            "Rental has same values in Units and PreparationTimeInDays. Nothing to change");
        _updateRentalAsyncNumberOfUnitsConflict = LoggerMessage.Define<int, int, int, DateTime>(
            LogLevel.Information,
            new EventId(103_005),
            "UpdateRentalAsync(id: {@id}, units: {@units}, preparationTimeInDays: {@preparationTimeInDays}) - " +
            "Preparation time in days makes conflict with bookings on date '{@date}");
        _updateRentalAsyncNumberOfPreparationTimeInDaysConflict = LoggerMessage.Define<int, int, int, DateTime, int>(
            LogLevel.Information,
            new EventId(103_006),
            "UpdateRentalAsync(id: {@id}, units: {@units}, preparationTimeInDays: {@preparationTimeInDays}) - " +
            "Units count too small for current bookings (bookings count for day {@date} is {bookingsCount}");
        _updateRentalAsyncDbConcurrencyExceptionWasThrown = LoggerMessage.Define<int, int, int, string>(
            LogLevel.Warning,
            new EventId(103_007),
            "UpdateRentalAsync(id: {@id}, units: {@units}, preparationTimeInDays: {@preparationTimeInDays}) - " +
            "DbConcurrenceException was thrown when was trying update rental. Error message: '{@errorMessage}'");
        _updateRentalAsyncEnd = LoggerMessage.Define<int, int, int>(
            LogLevel.Debug,
            new EventId(103_999),
            "UpdateRentalAsync(id: {@id}, units: {@units}, preparationTimeInDays: {@preparationTimeInDays}) - End");
    }

    // UpdateRentalAsync(int id, int units, int preparationTimeInDays)
    public static void UpdateRentalAsyncStart(this ILogger logger, int id, int units, int preparationTimeInDays)
    {
        _updateRentalAsyncStart(logger, id, units, preparationTimeInDays, null);
    }

    public static void UpdateRentalAsyncUnitsIsNegative(this ILogger logger, int id, int units, int preparationTimeInDays)
    {
        _updateRentalAsyncUnitsIsNegative(logger, id, units, preparationTimeInDays, null);
    }

    public static void UpdateRentalAsyncPreparationTimeInDaysIsNegative(this ILogger logger, int id, int units, int preparationTimeInDays)
    {
        _updateRentalAsyncPreparationTimeInDaysIsNegative(logger, id, units, preparationTimeInDays, null);
    }

    public static void UpdateRentalAsyncRentalNotFound(this ILogger logger, int id, int units, int preparationTimeInDays)
    {
        _updateRentalAsyncRentalNotFound(logger, id, units, preparationTimeInDays, null);
    }

    public static void UpdateRentalAsyncNothingToChange(this ILogger logger, int id, int units, int preparationTimeInDays)
    {
        _updateRentalAsyncNothingToChange(logger, id, units, preparationTimeInDays, null);
    }

    public static void UpdateRentalAsyncNumberOfUnitsConflict(
        this ILogger logger,
        int id,
        int units,
        int preparationTimeInDays,
        DateTime date)
    {
        _updateRentalAsyncNumberOfUnitsConflict(logger, id, units, preparationTimeInDays, date, null);
    }

    public static void UpdateRentalAsyncNumberOfPreparationTimeInDaysConflict(
        this ILogger logger,
        int id,
        int units,
        int preparationTimeInDays,
        DateTime date,
        int bookingsCount)
    {
        _updateRentalAsyncNumberOfPreparationTimeInDaysConflict(logger, id, units, preparationTimeInDays, date, bookingsCount, null);
    }
    
    public static void UpdateRentalAsyncDbConcurrencyExceptionWasThrown(
        this ILogger logger,
        int id,
        int units,
        int preparationTimeInDays,
        string errorMessage)
    {
        _updateRentalAsyncDbConcurrencyExceptionWasThrown(logger, id, units, preparationTimeInDays, errorMessage, null);
    }

    public static void UpdateRentalAsyncEnd(this ILogger logger, int id, int units, int preparationTimeInDays)
    {
        _updateRentalAsyncEnd(logger, id, units, preparationTimeInDays, null);
    }
}