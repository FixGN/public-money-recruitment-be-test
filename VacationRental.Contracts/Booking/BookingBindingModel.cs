namespace VacationRental.Contracts.Booking
{
    public class BookingBindingModel
    {
        public BookingBindingModel(int rentalId, DateTime start, int nights)
        {
            RentalId = rentalId;
            Start = start.Date;
            Nights = nights;
        }
        
        public int RentalId { get; }
        public DateTime Start { get; }
        public int Nights { get; }
    }
}
