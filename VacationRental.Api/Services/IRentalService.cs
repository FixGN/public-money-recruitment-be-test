using VacationRental.Api.Models;

namespace VacationRental.Api.Services;

public interface IRentalService
{
    public Rental? Get(int id);
    public Rental Create(int units);
}