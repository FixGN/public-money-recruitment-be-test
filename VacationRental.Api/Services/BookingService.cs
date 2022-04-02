using System;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Models;

namespace VacationRental.Api.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRentalRepository _rentalRepository;

    public BookingService(IBookingRepository bookingRepository, IRentalRepository rentalRepository)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
    }

    public Booking? GetBooking(int id)
    {
        return _bookingRepository.Get(id);
    }

    public CreateBookingResult CreateBooking(int rentalId, DateTime start, int nights)
    {
        var startDate = start.Date;

        var rental = _rentalRepository.Get(rentalId);
        if (rental == null)
        {
            return CreateBookingResult.Fail();
        }
        
        var currentBookings = _bookingRepository
            .GetByRentalId(rentalId)
            .Where(x => IsBookingsOverlap(x.Start, x.Nights, startDate, nights));

        if (rental.Units <= currentBookings.Count())
        {
            throw new ApplicationException("Not available");
        }
        
        var booking = _bookingRepository.Create(rentalId, startDate, nights);

        return CreateBookingResult.Successful(booking);
    }

    private static bool IsBookingsOverlap(DateTime firstStartDate, int firstNights, DateTime secondStartDate, int secondNights)
    {
        var firstEndDate = firstStartDate.AddDays(firstNights);
        var secondEndDate = secondStartDate.AddDays(secondNights);

        return firstStartDate <= secondStartDate && firstEndDate > secondStartDate
               || firstStartDate < secondEndDate && firstEndDate >= secondEndDate
               || firstStartDate > secondStartDate && firstEndDate < secondEndDate;
    }
}