using System.Data;
using System.Diagnostics.CodeAnalysis;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories.Dictionary;

public class DictionaryRentalRepository : IRentalRepository
{
    private readonly IDictionary<int, Rental> _repository;
    private readonly object _addLock = new();
    private readonly object _updateLock = new();

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
       
       lock (_addLock)
       {
           var rental = new Rental(_repository.Count + 1, units, preparationTimeInDays);
           _repository.Add(rental.Id, rental);

           return Task.FromResult(rental);
       }
    }

    public Task<Rental> UpdateAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        lock (_updateLock)
        {
            if (!_repository.ContainsKey(rental.Id))
            {
                throw new DBConcurrencyException($"Rental with Id {rental.Id} does not exist");
            }

            if (_repository[rental.Id].Version >= rental.Version)
            {
                throw new DBConcurrencyException($"Rental with Id {rental.Id} has been updated by another user");
            }

            _repository[rental.Id] = rental;
            return Task.FromResult(_repository[rental.Id]);
        }
    }
}