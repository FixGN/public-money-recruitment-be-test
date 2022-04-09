using System.Threading;
using System.Threading.Tasks;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories;

public interface IRentalRepository
{
    public Task<Rental?> GetOrDefaultAsync(int id, CancellationToken cancellationToken = default);
    public Task<Rental> CreateAsync(int units, int preparationTimeInDays, CancellationToken cancellationToken = default);
    public Task<Rental> UpdateAsync(Rental rental, CancellationToken cancellationToken = default);
}