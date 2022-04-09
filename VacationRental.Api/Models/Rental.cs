namespace VacationRental.Api.Models;

public class Rental
{
    public Rental(int id, int units, int preparationTimeInDays, int version = 1)
    {
        Id = id;
        Units = units;
        PreparationTimeInDays = preparationTimeInDays;
        Version = version;
    }

    public int Id { get; }
    public int Units { get; }
    public int PreparationTimeInDays { get; }
    public int Version { get; }
}