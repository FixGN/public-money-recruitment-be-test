namespace VacationRental.Api.Contracts.Calendar
{
    public class CalendarBookingViewModel
    {
        public CalendarBookingViewModel(int id, int unit)
        {
            Id = id;
            Unit = unit;
        }
        
        public int Id { get; }
        public int Unit { get; }
    }
}
