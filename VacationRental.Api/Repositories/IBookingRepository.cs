using System;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories;

public interface IBookingRepository
{
    public Booking? Get(int id);
    public Booking[] GetByRentalId(int rentalId);
    public Booking Create(int rentalId, DateTime start, int nights);
}