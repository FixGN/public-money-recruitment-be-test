using System;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Models;

namespace VacationRental.Api.Services.Implementation;

public class CalendarService : ICalendarService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRentalRepository _rentalRepository;

    public CalendarService(IBookingRepository bookingRepository, IRentalRepository rentalRepository)
    {
        _bookingRepository = bookingRepository;
        _rentalRepository = rentalRepository;
    }
    
    public GetCalendarDatesResult GetCalendarDates(int rentalId, DateTime start, int nights)
    {
        if (nights < 0)
            return GetCalendarDatesResult.Fail("Nights must be positive");
        // TODO: It's a part of contract? Can I delete this? 
        var rental = _rentalRepository.Get(rentalId);
        if (rental == null)
            return GetCalendarDatesResult.Fail("Rental not found");

        var calendarDates = new CalendarDate[nights];
        var availableBookings = _bookingRepository.GetByRentalId(rentalId);
        
        for (var i = 0; i < nights; i++)
        {
            var date = start.Date.AddDays(i);
            var bookings = availableBookings
                .Where(x => x.Start <= date && x.Start.AddDays(x.Nights) > date)
                .ToArray();

            calendarDates[i] = new CalendarDate(date, bookings);
        }

        return GetCalendarDatesResult.Success(calendarDates);
    }
}