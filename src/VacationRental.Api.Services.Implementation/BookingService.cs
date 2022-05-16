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
    private readonly ILogger<BookingService> _logger;
    private readonly IRentalRepository _rentalRepository;

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
        DateOnly startDate,
        int nights,
        CancellationToken cancellationToken = default)
    {
        _logger.CreateBookingAsyncStart(rentalId, startDate, nights);
        cancellationToken.ThrowIfCancellationRequested();

        if (nights <= 0)
        {
            _logger.CreateBookingAsyncNightsIsNegativeOrZero(rentalId, startDate, nights);
            return CreateBookingResult.ValidationFailed("Nights must be positive");
        }

        var rental = await _rentalRepository.GetOrDefaultAsync(rentalId, cancellationToken);
        if (rental == null)
        {
            _logger.CreateBookingAsyncRentalNotFound(rentalId, startDate, nights);
            return CreateBookingResult.ValidationFailed("Rental not found");
        }

        var currentBookings = await _bookingRepository.GetByRentalIdAndDatePeriodAsync(
            rentalId,
            startDate.AddDays(-rental.PreparationTimeInDays),
            startDate.AddDays(nights + rental.PreparationTimeInDays - 1),
            cancellationToken);

        var bookedUnits = currentBookings.Select(x => x.Unit).Distinct().Count();
        if (rental.Units <= bookedUnits)
        {
            _logger.CreateBookingAsyncAvailableUnitsNotFound(rentalId, startDate, nights);
            return CreateBookingResult.Conflict("No available rooms for the specified dates");
        }

        var booking = await _bookingRepository.CreateAsync(rentalId, startDate, nights, cancellationToken);

        _logger.CreateBookingAsyncEnd(rentalId, startDate, nights);
        return CreateBookingResult.Successful(booking);
    }
}