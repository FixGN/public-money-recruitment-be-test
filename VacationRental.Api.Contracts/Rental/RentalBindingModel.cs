namespace VacationRental.Api.Contracts.Rental
{
    public class RentalBindingModel
    {
        public RentalBindingModel(int units, int preparationTimeInDays)
        {
            Units = units;
            PreparationTimeInDays = preparationTimeInDays;
        }
        
        public int Units { get; }
        public int PreparationTimeInDays { get; }
    }
}
