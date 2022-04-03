using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories;

public class DictionaryBookingRepository : IBookingRepository
{
    private readonly Dictionary<int,Booking> _repository;

    public DictionaryBookingRepository(Dictionary<int, Booking> repository)
    {
        _repository = repository;
    }
    
    public Booking? Get(int id)
    {
        _repository.TryGetValue(id, out var booking);

        return booking;
    }

    public IEnumerable<Booking> GetByRentalId(int rentalId)
    {
        return _repository.Values
            .Where(x => x.RentalId == rentalId)
            .ToList();
    }

    public Booking Create(int rentalId, DateTime start, int nights)
    {
        var booking = new Booking(_repository.Count + 1, rentalId, start, nights);

        _repository.Add(booking.Id, booking);

        return booking;
    }
}