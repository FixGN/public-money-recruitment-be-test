using System;
using VacationRental.Api.Models;

namespace VacationRental.Api.ViewModels
{
    public class RentalViewModel
    {
        public RentalViewModel(int id, int units)
        {
            Id = id;
            Units = units;
        }
        
        public int Id { get; }
        public int Units { get; }

        public static RentalViewModel FromRental(Rental rental)
        {
            return new RentalViewModel(rental.Id, rental.Units);
        }
    }
}
