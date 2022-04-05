using System;

namespace VacationRental.Api.Models;

public class CalendarDate
{
    public CalendarDate(DateTime date, Booking[] bookings)
    {
        Date = date;
        Bookings = bookings;
    }

    public DateTime Date { get; }
    public Booking[] Bookings { get; }
}