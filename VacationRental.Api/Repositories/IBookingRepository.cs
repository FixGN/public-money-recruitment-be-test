using System;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories;

public interface IBookingRepository
{
    public Booking? GetOrDefault(int id);
    public Booking[] GetByRentalId(int rentalId);
    public Booking[] GetByRentalIdAndDatePeriod(int rentalId, DateTime startDate, DateTime endDate);
    public Booking Create(int rentalId, int unit, DateTime start, int nights);
}