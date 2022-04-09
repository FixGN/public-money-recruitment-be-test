using System;
using System.Threading;
using System.Threading.Tasks;
using VacationRental.Api.Services.Models;
using VacationRental.Api.Services.Models.Calendar;

namespace VacationRental.Api.Services;

public interface ICalendarService
{
    public Task<GetCalendarDatesResult> GetCalendarDatesAsync(
        int rentalId,
        DateTime start,
        int nights,
        CancellationToken cancellationToken = default);
}