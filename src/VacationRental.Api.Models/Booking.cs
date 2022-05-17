namespace VacationRental.Api.Models;

public class Booking
{
    public Booking(int id, int rentalId, int unit, DateOnly start, int nights)
    {
        Id = id;
        RentalId = rentalId;
        Unit = unit;
        Start = start;
        Nights = nights;
    }

    public int Id { get; }
    public int RentalId { get; }
    public int Unit { get; }
    public DateOnly Start { get; }
    public int Nights { get; }
}