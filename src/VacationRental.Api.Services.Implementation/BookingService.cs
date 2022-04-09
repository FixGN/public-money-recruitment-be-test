using Microsoft.Extensions.Logging;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Implementation.Logging.Extensions;
using VacationRental.Api.Services.Interfaces;
using VacationRental.Api.Services.Models.Booking;

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
        _logger.CreateBookingAsyncStart(rentalId, start, nights);
        cancellationToken.ThrowIfCancellationRequested();

        if (nights <= 0)
        {
            _logger.CreateBookingAsyncNightsIsNegativeOrZero(rentalId, start, nights);
            return CreateBookingResult.ValidationFailed("Nights must be positive");
        }
        var rental = await _rentalRepository.GetOrDefaultAsync(rentalId, cancellationToken);
        if (rental == null)
        {
            _logger.CreateBookingAsyncRentalNotFound(rentalId, start, nights);
            return CreateBookingResult.ValidationFailed("Rental not found");
        }
        
        var startDate = start.Date;

        var currentBookings = await _bookingRepository.GetByRentalIdAndDatePeriodAsync(
            rentalId,
            startDate.AddDays(-rental.PreparationTimeInDays),
            startDate.AddDays(nights + rental.PreparationTimeInDays - 1),
            cancellationToken);

        if (rental.Units <= currentBookings.Length)
        {
            _logger.CreateBookingAsyncAvailableUnitsNotFound(rentalId, start, nights);
            return CreateBookingResult.Conflict("No available rooms for the specified dates");
        }

        var booking = await _bookingRepository.CreateAsync(rentalId, startDate, nights, cancellationToken);

        _logger.CreateBookingAsyncEnd(rentalId, start, nights);
        return CreateBookingResult.Successful(booking);
    }
}