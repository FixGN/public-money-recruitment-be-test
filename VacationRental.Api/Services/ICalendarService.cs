using System;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services;

public interface ICalendarService
{
    public CalendarDate[] GetCalendarDates(int rentalId, DateTime start, int nights);
}