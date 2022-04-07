using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services.Models;

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
    
    public Rental? GetRentalOrDefault(int id)
    {
        return _rentalRepository.GetOrDefault(id);
    }

    public Rental CreateRental(int units, int preparationTimeInDays)
    {
        return _rentalRepository.Create(units, preparationTimeInDays);
    }

    public UpdateRentalResult UpdateRental(int id, int units, int preparationTimeInDays)
    {
        if (units < 0)
        {
            return UpdateRentalResult.ValidationFail("Units count must be positive number");
        }

        if (preparationTimeInDays < 0)
        {
            return UpdateRentalResult.ValidationFail("PreparationTime must be positive number");
        }

        var rental = _rentalRepository.GetOrDefault(id);
        if (rental == null)
        {
            return UpdateRentalResult.RentalNotFound(id);
        }

        if (rental.Units == units && rental.PreparationTimeInDays == preparationTimeInDays)
        {
            return UpdateRentalResult.Successful();
        }
        
        // TODO: Rewrite to async
        var rentalBookings = _bookingRepository.GetByRentalIdAsync(id).GetAwaiter().GetResult();
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

        _rentalRepository.Update(new Rental(rental.Id, units, preparationTimeInDays));
        return UpdateRentalResult.Successful();
    }
}