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

        foreach (var calendarDate1 in calendarDates1)
        {
            var calendarDate2 = calendarDates2.FirstOrDefault(x => x.Date == calendarDate1.Date);

            if (calendarDate2 == null)
            {
                return false;
            }

            if (calendarDate1.Bookings.Length != calendarDate2.Bookings.Length
                && calendarDate1.PreparationTimes.Length != calendarDate2.PreparationTimes.Length)
            {
                return false;
            }
            
            var bookingsAreEqual = calendarDate1.Bookings
                .All(x => calendarDate2.Bookings.Any(x.AreEqual));

            if (!bookingsAreEqual)
            {
                return false;
            }

            var preparationTimesAreEqual = calendarDate1.PreparationTimes
                .All(pt1 => calendarDate2.PreparationTimes.Any(pt2 => pt1.Unit == pt2.Unit));

            if (!preparationTimesAreEqual)
            {
                return false;
            }
        }

        return true;
    }
}