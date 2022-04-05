using System;

namespace VacationRental.Api.Models;

public class CalendarDate
{
    public CalendarDate(DateTime date, Booking[] bookings, CalendarPreparationTime[] preparationDates)
    {
        Date = date;
        Bookings = bookings ?? throw new ArgumentNullException(nameof(bookings));
        PreparationTimes = preparationDates ?? throw new ArgumentNullException(nameof(preparationDates));
    }

    public DateTime Date { get; }
    public Booking[] Bookings { get; }
    public CalendarPreparationTime[] PreparationTimes { get; }
}