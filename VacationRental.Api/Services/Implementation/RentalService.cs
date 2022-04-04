using VacationRental.Api.Models;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Services.Implementation;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;

    public RentalService(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }
    
    public Rental? GetRentalOrDefault(int id)
    {
        return _rentalRepository.GetOrDefault(id);
    }

    public Rental CreateRental(int units)
    {
        return _rentalRepository.Create(units);
    }
}