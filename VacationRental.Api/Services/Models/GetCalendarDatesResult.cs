using System;
using System.Diagnostics.CodeAnalysis;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services.Models;

public class GetCalendarDatesResult
{
    private GetCalendarDatesResult(bool isSuccess, string? errorMessage, CalendarDate[] calendarDates)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        CalendarDates = calendarDates;
    }

    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public CalendarDate[] CalendarDates { get; }
    
    public static GetCalendarDatesResult Success(CalendarDate[] calendarDates)
    {
        return new GetCalendarDatesResult(true, null, calendarDates);
    }

    public static GetCalendarDatesResult Fail(string errorMessage)
    {
        return new GetCalendarDatesResult(false, errorMessage, Array.Empty<CalendarDate>());
    }
}