using System;
using VacationRental.Api.Models;

namespace VacationRental.Api.ViewModels
{
    public class BookingViewModel
    {
        public BookingViewModel(int id, int rentalId, DateTime start, int nights)
        {
            Id = id;
            RentalId = rentalId;
            Start = start;
            Nights = nights;
        }
        
        public int Id { get; }
        public int RentalId { get; }
        public DateTime Start { get; }
        public int Nights { get; }
        
        public static BookingViewModel FromBooking(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking));
            }
            
            return new BookingViewModel(booking.Id, booking.RentalId, booking.Start, booking.Nights);
        }
    }
}
