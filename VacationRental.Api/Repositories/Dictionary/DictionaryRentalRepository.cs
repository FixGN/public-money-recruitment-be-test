using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories.Dictionary;

public class DictionaryRentalRepository : IRentalRepository
{
    private readonly IDictionary<int, Rental> _repository;
    private readonly object _lock = new();

    public DictionaryRentalRepository(IDictionary<int, Rental> repository)
    {
        _repository = repository;
    }

    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    public Rental? GetOrDefault(int id)
    {
        _repository.TryGetValue(id, out var rental);
        return rental;
    }

    public Rental Create(int units, int preparationTimeInDays)
    {
        lock (_lock)
        {
            var rental = new Rental(_repository.Count + 1, units, preparationTimeInDays);
            _repository.Add(rental.Id, rental);

            return rental;
        }
    }

    public void Update(Rental rental)
    {
        lock (_lock)
        {
            if (_repository.TryGetValue(rental.Id, out _))
            {
                _repository[rental.Id] = rental;
            }
            else
            {
                throw new DBConcurrencyException($"Rental with Id {rental.Id} does not exist");
            }
        }
    }
}