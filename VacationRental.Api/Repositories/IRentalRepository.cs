using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories;

public interface IRentalRepository
{
    public Rental? GetOrDefault(int id);
    public Rental Create(int units, int preparationTimeInDays);
}