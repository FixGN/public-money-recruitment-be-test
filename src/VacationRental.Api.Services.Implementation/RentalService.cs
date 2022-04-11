using System.Data;
using Microsoft.Extensions.Logging;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Implementation.Logging.Extensions;
using VacationRental.Api.Services.Interfaces;
using VacationRental.Api.Services.Models.Rental;

namespace VacationRental.Api.Services.Implementation;

public class RentalService : IRentalService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly ILogger<RentalService> _logger;

    public RentalService(IRentalRepository rentalRepository, IBookingRepository bookingRepository, ILogger<RentalService> logger)
    {
        _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Rental?> GetRentalOrDefaultAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _rentalRepository.GetOrDefaultAsync(id, cancellationToken);
    }

    public async Task<CreateRentalResult> CreateRentalAsync(
        int units,
        int preparationTimeInDays,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (units < 0)
        {
            return CreateRentalResult.ValidationFailed("Units must be positive number");
        }

        if (preparationTimeInDays < 0)
        {
            return CreateRentalResult.ValidationFailed("Preparation time must be positive number");
        }

        var createdRental = await _rentalRepository.CreateAsync(units, preparationTimeInDays, cancellationToken);

        return CreateRentalResult.Successful(createdRental);
    }

    public async Task<UpdateRentalResult> UpdateRentalAsync(
        int id,
        int units,
        int preparationTimeInDays,
        CancellationToken cancellationToken = default)
    {
        _logger.UpdateRentalAsyncStart(id, units, preparationTimeInDays);
        cancellationToken.ThrowIfCancellationRequested();

        if (units < 0)
        {
            _logger.UpdateRentalAsyncUnitsIsNegative(id, units, preparationTimeInDays);
            return UpdateRentalResult.ValidationFailed("Units count must be positive number");
        }

        if (preparationTimeInDays < 0)
        {
            _logger.UpdateRentalAsyncPreparationTimeInDaysIsNegative(id, units, preparationTimeInDays);
            return UpdateRentalResult.ValidationFailed("PreparationTime must be positive number");
        }

        var rental = await _rentalRepository.GetOrDefaultAsync(id, cancellationToken);
        if (rental == null)
        {
            _logger.UpdateRentalAsyncRentalNotFound(id, units, preparationTimeInDays);
            return UpdateRentalResult.RentalNotFound(id);
        }

        if (rental.Units == units && rental.PreparationTimeInDays == preparationTimeInDays)
        {
            _logger.UpdateRentalAsyncNothingToChange(id, units, preparationTimeInDays);
            return UpdateRentalResult.Successful(rental);
        }

        var rentalBookings = await _bookingRepository.GetByRentalIdAsync(id, cancellationToken);
        if (rentalBookings.Length != 0)
        {
            var minRentalDate = rentalBookings.Select(x => x.Start).Min();
            var maxRentalDate = rentalBookings.Select(x => x.Start.AddDays(x.Nights + rental.PreparationTimeInDays - 1)).Max();
            var currentBookingDaysCount = maxRentalDate.DayNumber - minRentalDate.DayNumber + 1;

            for (var i = 0; i < currentBookingDaysCount; i++)
            {
                var date = minRentalDate.AddDays(i);
                var bookingsWithNewPreparationTime = rentalBookings
                    .Where(x => x.Start <= date
                                && date <= x.Start.AddDays(x.Nights + preparationTimeInDays - 1))
                    .ToList();

                if (bookingsWithNewPreparationTime.Count != bookingsWithNewPreparationTime.Select(x => x.Unit).Distinct().Count())
                {
                    _logger.UpdateRentalAsyncNumberOfUnitsConflict(id, units, preparationTimeInDays, date);
                    return UpdateRentalResult.Conflict(
                        $"Preparation time in days '{preparationTimeInDays}' makes conflict with bookings on date '{date:d}'");
                }

                if (units < bookingsWithNewPreparationTime.Count)
                {
                    _logger.UpdateRentalAsyncNumberOfPreparationTimeInDaysConflict(
                        id,
                        units,
                        preparationTimeInDays,
                        date,
                        bookingsWithNewPreparationTime.Count);
                    return UpdateRentalResult.Conflict(
                        $"Units count too small for current bookings (bookings count for day {date:d} is {bookingsWithNewPreparationTime.Count})");
                }
            }
        }

        var updatedRental = new Rental(rental.Id, units, preparationTimeInDays, rental.Version + 1);
        try
        {
            await _rentalRepository.UpdateAsync(updatedRental, cancellationToken);
        }
        catch (DBConcurrencyException e)
        {
            _logger.UpdateRentalAsyncDbConcurrencyExceptionWasThrown(id, units, preparationTimeInDays, e.Message);
            return UpdateRentalResult.ConcurrencyException(e.Message);
        }
        _logger.UpdateRentalAsyncEnd(id, units, preparationTimeInDays);
        return UpdateRentalResult.Successful(updatedRental);
    }
}