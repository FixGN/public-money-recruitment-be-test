using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public Booking? GetOrDefault(int id)
    {
        _repository.TryGetValue(id, out var booking);

        return booking;
    }

    public Booking[] GetByRentalIdAndDatePeriod(int rentalId, DateTime startDate, DateTime endDate)
    {
        return _repository.Values
            .Where(x =>
            {
                var currentEndDate = x.Start.AddDays(x.Nights - 1);
                return x.RentalId == rentalId
                       && x.Start <= endDate.Date
                       && startDate.Date <= currentEndDate;
            })
            .ToArray();
    }

    public Booking Create(int rentalId, DateTime start, int nights)
    {
        lock (_lock)
        {
            var booking = new Booking(_repository.Count + 1, rentalId, start, nights);
            _repository.Add(booking.Id, booking);

            return booking;
        }
    }
}