using System.Collections.Generic;
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
}