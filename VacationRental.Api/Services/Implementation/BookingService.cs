using System;
using System.Collections.Generic;
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

    public Booking? GetBookingOrDefault(int id)
    {
        return _bookingRepository.GetOrDefault(id);
    }

    public CreateBookingResult CreateBooking(int rentalId, DateTime start, int nights)
    {
        _logger.CreateBookingStart(rentalId, start, nights);
        if (nights <= 0)
        {
            _logger.CreateBookingNightsIsNegativeOrZero(rentalId, start, nights);
            return CreateBookingResult.ValidationFail("Nights must be positive");
        }
        var rental = _rentalRepository.GetOrDefault(rentalId);
        if (rental == null)
        {
            _logger.CreateBookingRentalNotFound(rentalId, start, nights);
            return CreateBookingResult.ValidationFail("Rental not found");
        }
        
        var startDate = start.Date;

        // TODO: Make test for preparationTimeInDays period (can't create booking, if unit in preparation)
        var currentBookings = _bookingRepository
            .GetByRentalIdAndDatePeriod(rentalId, startDate.AddDays(-rental.PreparationTimeInDays), startDate.AddDays(nights + rental.PreparationTimeInDays - 1));

        if (rental.Units <= currentBookings.Length)
        {
            _logger.CreateBookingAvailableUnitsNotFound(rentalId, start, nights);
            return CreateBookingResult.Conflict("No available rooms for the specified dates");
        }
        
        // In current implementation - ok, but better get available unit and create booking in one transaction
        // TODO: Think about one transaction for that
        var bookedUnits = currentBookings.Select(x => x.Unit);
        var availableUnit = GetFirstAvailableUnit(bookedUnits, rental.Units);

        var booking = _bookingRepository.Create(rentalId, availableUnit, startDate, nights);

        _logger.CreateBookingEnd(rentalId, start, nights);
        return CreateBookingResult.Successful(booking);
    }
    
    private static int GetFirstAvailableUnit(IEnumerable<int> bookedUnits, int unitsCount) 
        => Enumerable.Range(1, unitsCount).Except(bookedUnits).Min();
}