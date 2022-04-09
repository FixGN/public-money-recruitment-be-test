using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace VacationRental.Api.Contracts.v1.Booking
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class BookingBindingModel
    {
        public BookingBindingModel(int rentalId, DateTime start, int nights)
        {
            if (rentalId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rentalId),"Rental id cannot be negative");
            }

            if (nights <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nights),"Nights must be greater than 0");
            }

            RentalId = rentalId;
            Start = start.Date;
            Nights = nights;
        }
        
        /// <summary>Parameterless constructor for System.Text.Json.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public BookingBindingModel()
        {
        }
        
        [Range(0, int.MaxValue)]
        public int RentalId { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Nights { get; set; }
    }
}