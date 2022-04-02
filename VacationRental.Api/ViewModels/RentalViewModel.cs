using VacationRental.Api.Models;

namespace VacationRental.Api.ViewModels
{
    public class RentalViewModel
    {
        public int Id { get; set; }
        public int Units { get; set; }

        public static RentalViewModel FromRental(Rental rental)
        {
            return new RentalViewModel
            {
                Id = rental.Id,
                Units = rental.Units
            };
        }
    }
}
