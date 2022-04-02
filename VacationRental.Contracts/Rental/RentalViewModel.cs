namespace VacationRental.Contracts.Rental
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
    }
}
