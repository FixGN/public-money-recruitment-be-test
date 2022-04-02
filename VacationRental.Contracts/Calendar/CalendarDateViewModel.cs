namespace VacationRental.Contracts.Calendar
{
    public class CalendarDateViewModel
    {
        public CalendarDateViewModel(DateTime date, List<CalendarBookingViewModel> bookings)
        {
            Date = date;
            Bookings = bookings ?? throw new ArgumentNullException(nameof(bookings));
        }
        
        public DateTime Date { get; }
        public List<CalendarBookingViewModel> Bookings { get; }
    }
}
