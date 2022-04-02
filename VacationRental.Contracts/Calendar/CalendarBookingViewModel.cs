namespace VacationRental.Contracts.Calendar
{
    public class CalendarBookingViewModel
    {
        public CalendarBookingViewModel(int id)
        {
            Id = id;
        }
        
        public int Id { get; }
    }
}
