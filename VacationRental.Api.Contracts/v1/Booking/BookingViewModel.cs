namespace VacationRental.Api.Contracts.Booking
{
    public class BookingViewModel
    {
        public BookingViewModel(int id, int rentalId, int unit, DateTime start, int nights)
        {
            Id = id;
            RentalId = rentalId;
            Unit = unit;
            Start = start;
            Nights = nights;
        }
        
        public int Id { get; }
        public int RentalId { get; }
        public int Unit { get; }
        public DateTime Start { get; }
        public int Nights { get; }
    }
}
