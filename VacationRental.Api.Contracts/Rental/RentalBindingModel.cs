namespace VacationRental.Api.Contracts.Rental
{
    public class RentalBindingModel
    {
        public RentalBindingModel(int units)
        {
            Units = units;
        }
        
        public int Units { get; }
    }
}
