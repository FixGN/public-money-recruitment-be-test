using System.Threading;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using VacationRental.Api.Services.Models;

namespace VacationRental.Api.Services;

public interface IRentalService
{
    public Task<Rental?> GetRentalOrDefaultAsync(int id, CancellationToken cancellationToken = default);
    public Task<Rental> CreateRentalAsync(int units, int preparationTimeInDays, CancellationToken cancellationToken = default);
    public Task<UpdateRentalResult> UpdateRentalAsync(
        int id,
        int units,
        int preparationTimeInDays,
        CancellationToken cancellationToken = default);
}