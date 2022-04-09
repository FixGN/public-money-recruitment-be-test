using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    public async Task<Booking?> GetBookingOrDefaultAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return await _bookingRepository.GetOrDefaultAsync(id, cancellationToken);
    }

    public async Task<CreateBookingResult> CreateBookingAsync(
        int rentalId,
        DateTime start,
        int nights,
        CancellationToken cancellationToken = default)
    {
        _logger.CreateBookingStart(rentalId, start, nights);
        cancellationToken.ThrowIfCancellationRequested();

        if (nights <= 0)
        {
            _logger.CreateBookingNightsIsNegativeOrZero(rentalId, start, nights);
            return CreateBookingResult.ValidationFail("Nights must be positive");
        }
        var rental = await _rentalRepository.GetOrDefaultAsync(rentalId, cancellationToken);
        if (rental == null)
        {
            _logger.CreateBookingRentalNotFound(rentalId, start, nights);
            return CreateBookingResult.ValidationFail("Rental not found");
        }
        
        var startDate = start.Date;

        var currentBookings = await _bookingRepository.GetByRentalIdAndDatePeriodAsync(
            rentalId,
            startDate.AddDays(-rental.PreparationTimeInDays),
            startDate.AddDays(nights + rental.PreparationTimeInDays - 1),
            cancellationToken);

        if (rental.Units <= currentBookings.Length)
        {
            _logger.CreateBookingAvailableUnitsNotFound(rentalId, start, nights);
            return CreateBookingResult.Conflict("No available rooms for the specified dates");
        }

        var booking = await _bookingRepository.CreateAsync(rentalId, startDate, nights, cancellationToken);

        _logger.CreateBookingEnd(rentalId, start, nights);
        return CreateBookingResult.Successful(booking);
    }
}