using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories;

public interface IBookingRepository
{
    public Task<Booking?> GetOrDefaultAsync(int id, CancellationToken cancellationToken = default);
    public Task<Booking[]> GetByRentalIdAsync(int rentalId, CancellationToken cancellationToken = default);

    public Task<Booking[]> GetByRentalIdAndDatePeriodAsync(int rentalId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);

    public Task<Booking> CreateAsync(int rentalId, DateOnly start, int nights, CancellationToken cancellationToken = default);
}