using System;
using System.Threading;
using System.Threading.Tasks;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories;

public interface IBookingRepository
{
    public Task<Booking?> GetOrDefaultAsync(int id, CancellationToken cancellationToken = default);
    public Task<Booking[]> GetByRentalIdAsync(int rentalId, CancellationToken cancellationToken = default);
    public Task<Booking[]> GetByRentalIdAndDatePeriodAsync(
        int rentalId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    public Task<Booking> CreateAsync(int rentalId, int unit, DateTime start, int nights, CancellationToken cancellationToken = default);
}