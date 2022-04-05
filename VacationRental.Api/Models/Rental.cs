namespace VacationRental.Api.Models;

public class Rental
{
    public Rental(int id, int units, int preparationTimeInDays)
    {
        Id = id;
        Units = units;
        PreparationTimeInDays = preparationTimeInDays;
    }

    public int Id { get; }
    public int Units { get; }
    public int PreparationTimeInDays { get; }
}