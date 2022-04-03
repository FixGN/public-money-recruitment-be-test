using VacationRental.Api.Services.Models;

namespace VacationRental.Api.Tests.Unit.Extensions;

public static class GetCalendarDatesResultExtension
{
    public static bool AreEqual(this GetCalendarDatesResult result1, GetCalendarDatesResult result2)
    {
        return result1.IsSuccess == result2.IsSuccess
               && result1.ErrorMessage == result2.ErrorMessage
               && result1.CalendarDates.AreEqual(result2.CalendarDates);
    }
}