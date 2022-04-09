using System;
using System.Threading;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using VacationRental.Api.Services.Models;
using VacationRental.Api.Services.Models.Booking;

namespace VacationRental.Api.Services;

public interface IBookingService
{
    public Task<Booking?> GetBookingOrDefaultAsync(int id, CancellationToken cancellationToken = default);
    public Task<CreateBookingResult> CreateBookingAsync(
        int rentalId,
        DateTime start,
        int nights,
        CancellationToken cancellationToken = default);
}