namespace VacationRental.Api.Contracts.v1.Calendar;

public class CalendarDateViewModel
{
    public CalendarDateViewModel(
        DateTime date,
        List<CalendarBookingViewModel> bookings,
        List<CalendarPreparationTimeViewModel> preparationTimes)
    {
        Date = date;
        Bookings = bookings ?? throw new ArgumentNullException(nameof(bookings));
        PreparationTimes = preparationTimes ?? throw new ArgumentNullException(nameof(preparationTimes));
    }

    public DateTime Date { get; }
    public List<CalendarBookingViewModel> Bookings { get; }
    public List<CalendarPreparationTimeViewModel> PreparationTimes { get; }
}