namespace VacationRental.Api.Models;

public class CalendarDate
{
    public CalendarDate(DateOnly date, Booking[] bookings, CalendarPreparationTime[] preparationDates)
    {
        Date = date;
        Bookings = bookings ?? throw new ArgumentNullException(nameof(bookings));
        PreparationTimes = preparationDates ?? throw new ArgumentNullException(nameof(preparationDates));
    }

    public DateOnly Date { get; }
    public Booking[] Bookings { get; }
    public CalendarPreparationTime[] PreparationTimes { get; }
}