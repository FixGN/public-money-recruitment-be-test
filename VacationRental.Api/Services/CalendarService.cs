using System;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Services;

public class CalendarService : ICalendarService
{
    private readonly IBookingRepository _bookingRepository;

    public CalendarService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }
    
    public CalendarDate[] GetCalendarDates(int rentalId, DateTime start, int nights)
    {
        var calendarDateDates = new CalendarDate[nights];
        
        for (var i = 0; i < nights; i++)
        {
            var date = start.Date.AddDays(i);
            var bookings = _bookingRepository
                // TODO: Can be null
                .GetByRentalId(rentalId)
                .Where(x => x.Start <= date && x.Start.AddDays(x.Nights) > date)
                .ToArray();

            calendarDateDates[i] = new CalendarDate(date, bookings);
        }

        return calendarDateDates;
    }
}