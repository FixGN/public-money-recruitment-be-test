using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories;

public interface IRentalRepository
{
    Rental? GetOrDefault(int id);
    Rental Create(int units);
}