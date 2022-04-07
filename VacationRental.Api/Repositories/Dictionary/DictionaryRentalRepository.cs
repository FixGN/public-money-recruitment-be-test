using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
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
    public Task<Rental?> GetOrDefaultAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _repository.TryGetValue(id, out var rental);
        return Task.FromResult(rental);
    }

    public Task<Rental> CreateAsync(int units, int preparationTimeInDays, CancellationToken cancellationToken = default)
    {
       cancellationToken.ThrowIfCancellationRequested();
       
       lock (_lock)
       {
           var rental = new Rental(_repository.Count + 1, units, preparationTimeInDays);
           _repository.Add(rental.Id, rental);

           return Task.FromResult(rental);
       }
    }

    public Task UpdateAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        lock (_lock)
        {
            if (_repository.TryGetValue(rental.Id, out _))
            {
                Task.FromResult(_repository[rental.Id] = rental);
            }
            else
            {
                throw new DBConcurrencyException($"Rental with Id {rental.Id} does not exist");
            }
        }

        return Task.CompletedTask;
    }
}