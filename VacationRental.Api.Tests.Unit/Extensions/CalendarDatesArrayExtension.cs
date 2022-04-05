using System.Linq;
using VacationRental.Api.Models;

namespace VacationRental.Api.Tests.Unit.Extensions;

public static class CalendarDatesArrayExtension
{
    public static bool AreEqual(this CalendarDate[] calendarDates1, CalendarDate[] calendarDates2)
    {
        if (calendarDates1.Length != calendarDates2.Length)
        {
            return false;
        }

        return !calendarDates1
            .Where((calendarDate, i) => calendarDate.Date != calendarDates2[i].Date)
            .Any();
    }
}