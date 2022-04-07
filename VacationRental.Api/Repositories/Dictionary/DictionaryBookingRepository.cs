using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories.Dictionary;

public class DictionaryBookingRepository : IBookingRepository
{
    private readonly IDictionary<int,Booking> _repository;
    private readonly object _lock = new();

    public DictionaryBookingRepository(IDictionary<int, Booking> repository)
    {
        _repository = repository;
    }
    
    public Task<Booking?> GetOrDefaultAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _repository.TryGetValue(id, out var booking);

        return Task.FromResult(booking);
    }

    public Task<Booking[]> GetByRentalIdAsync(int rentalId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return Task.FromResult(_repository.Values.Where(x => x.RentalId == rentalId).ToArray());
    }

    public Task<Booking[]> GetByRentalIdAndDatePeriodAsync(
        int rentalId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return Task.FromResult(_repository.Values
            .Where(x =>
            {
                var currentEndDate = x.Start.AddDays(x.Nights - 1);
                return x.RentalId == rentalId
                       && x.Start <= endDate.Date
                       && startDate.Date <= currentEndDate;
            })
            .ToArray());
    }

    public Task<Booking> CreateAsync(int rentalId, int unit, DateTime start, int nights, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        lock (_lock)
        {
            var booking = new Booking(_repository.Count + 1, rentalId, unit, start, nights);
            _repository.Add(booking.Id, booking);

            return Task.FromResult(booking);
        }
    }
}