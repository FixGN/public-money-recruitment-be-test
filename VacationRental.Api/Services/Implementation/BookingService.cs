using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using VacationRental.Api.Logging.Extensions.Services;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Models;

namespace VacationRental.Api.Services.Implementation;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IBookingRepository bookingRepository, IRentalRepository rentalRepository, ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Booking? GetBooking(int id)
    {
        return _bookingRepository.Get(id);
    }

    public CreateBookingResult CreateBooking(int rentalId, DateTime start, int nights)
    {
        _logger.CreateBookingStart(rentalId, start, nights);
        if (nights <= 0)
        {
            _logger.CreateBookingNightsIsNegativeOrZero(rentalId, start, nights);
            return CreateBookingResult.ValidationFail("Nights must be positive");
        }
        var rental = _rentalRepository.Get(rentalId);
        if (rental == null)
        {
            _logger.CreateBookingRentalNotFound(rentalId, start, nights);
            return CreateBookingResult.ValidationFail("Rental not found");
        }
        
        var startDate = start.Date;
        
        var currentBookings = _bookingRepository
            .GetByRentalId(rentalId)
            .Where(x => IsBookingsOverlap(x.Start, x.Nights, startDate, nights));

        if (rental.Units <= currentBookings.Count())
        {
            _logger.CreateBookingAvailableUnitsNotFound(rentalId, start, nights);
            return CreateBookingResult.Conflict("Not available");
        }
        
        var booking = _bookingRepository.Create(rentalId, startDate, nights);

        _logger.CreateBookingEnd(rentalId, start, nights);
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