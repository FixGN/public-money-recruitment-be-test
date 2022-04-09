namespace VacationRental.Api.Contracts.v1.Rental;

public class RentalViewModel
{
    public RentalViewModel(int id, int units, int preparationTimeInDays)
    {
        Id = id;
        Units = units;
        PreparationTimeInDays = preparationTimeInDays;
    }

    public int Id { get; }
    public int Units { get; }
    public int PreparationTimeInDays { get; }
}