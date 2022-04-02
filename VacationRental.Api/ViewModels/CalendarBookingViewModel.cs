using VacationRental.Api.Models;

namespace VacationRental.Api.ViewModels
{
    public class CalendarBookingViewModel
    {
        public CalendarBookingViewModel(int id)
        {
            Id = id;
        }
        
        public int Id { get; }
        
        public static CalendarBookingViewModel FromBooking(Booking booking)
        {
            return new CalendarBookingViewModel(booking.Id);
        }
    }
}
