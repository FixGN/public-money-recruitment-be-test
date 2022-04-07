using System;
using System.Threading;
using System.Threading.Tasks;
using VacationRental.Api.Services.Models;

namespace VacationRental.Api.Services;

public interface ICalendarService
{
    public Task<GetCalendarDatesResult> GetCalendarDatesAsync(
        int rentalId,
        DateTime start,
        int nights,
        CancellationToken cancellationToken = default);
}