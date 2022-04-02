namespace VacationRental.Api.Contracts.Booking
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
    }
}
