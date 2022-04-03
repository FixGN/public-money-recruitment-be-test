using System;
using VacationRental.Api.Services.Models;

namespace VacationRental.Api.Services;

public interface ICalendarService
{
    public GetCalendarDatesResult GetCalendarDates(int rentalId, DateTime start, int nights);
}