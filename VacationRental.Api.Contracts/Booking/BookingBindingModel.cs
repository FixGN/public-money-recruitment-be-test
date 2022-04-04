using System.ComponentModel.DataAnnotations;

namespace VacationRental.Api.Contracts.Booking
{
    public class BookingBindingModel
    {
        public BookingBindingModel(int rentalId, DateTime start, int nights)
        {
            if (rentalId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rentalId),"Rental id cannot be negative");
            }

            if (nights < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nights),"Nights cannot be negative");
            }

            RentalId = rentalId;
            Start = start.Date;
            Nights = nights;
        }
        
        [Range(0, int.MaxValue)]
        public int RentalId { get; }
        [Required]
        public DateTime Start { get; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Nights { get; }
    }
}