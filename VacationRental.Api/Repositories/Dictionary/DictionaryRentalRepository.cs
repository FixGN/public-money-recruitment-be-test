using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories.Dictionary;

public class DictionaryRentalRepository : IRentalRepository
{
    private readonly IDictionary<int, Rental> _repository;

    public DictionaryRentalRepository(IDictionary<int, Rental> repository)
    {
        _repository = repository;
    }

    public Rental? GetOrDefault(int id)
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