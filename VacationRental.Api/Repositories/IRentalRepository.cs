using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories;

public interface IRentalRepository
{
    Rental? Get(int id);
    Rental Create(int units);
}