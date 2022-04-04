using VacationRental.Api.Models;

namespace VacationRental.Api.Services;

public interface IRentalService
{
    public Rental? GetRental(int id);
    public Rental CreateRental(int units, int preparationTimeInDays);
}