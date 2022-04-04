using System.ComponentModel.DataAnnotations;

namespace VacationRental.Api.Contracts.Rental
{
    public class RentalBindingModel
    {
        public RentalBindingModel(int units)
        {
            if (units < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(units), "Units must be greater than 0");
            }

            Units = units;
        }
        
        [Range(0, int.MaxValue)]
        public int Units { get; }
    }
}