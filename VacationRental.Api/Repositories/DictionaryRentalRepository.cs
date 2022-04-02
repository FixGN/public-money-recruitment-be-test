using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories;

public class DictionaryRentalRepository : IRentalRepository
{
    private readonly IDictionary<int, Rental> _repository;

    public DictionaryRentalRepository()
    {
        _repository = new Dictionary<int, Rental>();
    }
    
    public Rental? Get(int id)
    {
        _repository.TryGetValue(id, out var rental);
        return rental;
    }

    public Rental Create(int units)
    {
        var rental = new Rental(_repository.Count + 1, units);

        _repository.Add(rental.Id, rental);

        return rental;
    }
}