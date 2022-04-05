namespace VacationRental.Api.Models;

public class Rental
{
    public Rental(int id, int units)
    {
        Id = id;
        Units = units;
    }

    public int Id { get; }
    public int Units { get; }
}