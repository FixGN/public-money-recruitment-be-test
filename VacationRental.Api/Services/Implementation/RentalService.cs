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
    
    public Rental? Get(int id)
    {
        return _rentalRepository.Get(id);
    }

    public Rental Create(int units)
    {
        return _rentalRepository.Create(units);
    }
}