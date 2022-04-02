namespace VacationRental.Api.ViewModels
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
