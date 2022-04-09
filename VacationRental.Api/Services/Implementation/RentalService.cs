using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Models;
using VacationRental.Api.Services.Models.Rental;

namespace VacationRental.Api.Services.Implementation;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IBookingRepository _bookingRepository;

    public RentalService(IRentalRepository rentalRepository, IBookingRepository bookingRepository)
    {
        _rentalRepository = rentalRepository;
        _bookingRepository = bookingRepository;
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
            return CreateRentalResult.ValidationFail("Units must be positive number");
        }
        if (preparationTimeInDays < 0)
        {
            return CreateRentalResult.ValidationFail("Preparation time must be positive number");
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
        cancellationToken.ThrowIfCancellationRequested();
        
        if (units < 0)
        {
            return UpdateRentalResult.ValidationFail("Units count must be positive number");
        }

        if (preparationTimeInDays < 0)
        {
            return UpdateRentalResult.ValidationFail("PreparationTime must be positive number");
        }

        var rental = await _rentalRepository.GetOrDefaultAsync(id, cancellationToken);
        if (rental == null)
        {
            return UpdateRentalResult.RentalNotFound(id);
        }

        if (rental.Units == units && rental.PreparationTimeInDays == preparationTimeInDays)
        {
            return UpdateRentalResult.Successful();
        }
        
        var rentalBookings = await _bookingRepository.GetByRentalIdAsync(id, cancellationToken);
        if (rentalBookings.Length != 0)
        {
            var minRentalDate = rentalBookings.Select(x => x.Start).Min();
            var maxRentalDate = rentalBookings.Select(x => x.Start.AddDays(x.Nights + rental.PreparationTimeInDays - 1)).Max();
            var currentBookingDaysCount = (maxRentalDate - minRentalDate).Days + 1;

            for(var i = 0; i < currentBookingDaysCount; i++)
            {
                var date = minRentalDate.AddDays(i);
                var bookingsWithNewPreparationTime = rentalBookings
                    .Where(x => x.Start <= date
                                && date <= x.Start.AddDays(x.Nights + preparationTimeInDays - 1))
                    .ToList();

                if (bookingsWithNewPreparationTime.Count != bookingsWithNewPreparationTime.Select(x => x.Unit).Distinct().Count())
                {
                    return UpdateRentalResult.Conflict(
                        $"Preparation time in days '{preparationTimeInDays}' makes conflict with bookings on date '{date:d}'");
                }
                if (units < bookingsWithNewPreparationTime.Count)
                {
                    return UpdateRentalResult.Conflict(
                        $"Units count too small for current bookings (bookings count for day {date:d} is {bookingsWithNewPreparationTime.Count})");
                }
            }
        }

        await _rentalRepository.UpdateAsync(new Rental(rental.Id, units, preparationTimeInDays), cancellationToken);
        return UpdateRentalResult.Successful();
    }
}