using System;
using VacationRental.Api.Models;
using VacationRental.Api.Services.Models;

namespace VacationRental.Api.Services;

public interface IBookingService
{
    public Booking? GetBooking(int id);
    public CreateBookingResult CreateBooking(int rentalId, DateTime start, int nights);
}