using VacationRental.Api.Models;
using VacationRental.Api.Services.Models;

namespace VacationRental.Api.Services;

public interface IRentalService
{
    public Rental? GetRentalOrDefault(int id);
    public Rental CreateRental(int units, int preparationTimeInDays);
    public UpdateRentalResult UpdateRental(int id, int units, int preparationTimeInDays);
}