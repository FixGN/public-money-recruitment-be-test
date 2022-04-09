using VacationRental.Api.Services.Models.Calendar;

namespace VacationRental.Api.Services.Interfaces;

public interface ICalendarService
{
    public Task<GetCalendarDatesResult> GetCalendarDatesAsync(
        int rentalId,
        DateTime start,
        int nights,
        CancellationToken cancellationToken = default);
}