using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Contracts.v1.Common;
using VacationRental.Api.Controllers.v1.Mappers;
using VacationRental.Api.Services.Interfaces;

namespace VacationRental.Api.Controllers.v1;

[Route("api/v1/calendar")]
[ApiController]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;

    public CalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService ?? throw new ArgumentNullException(nameof(calendarService));
    }

    [HttpGet]
    public async Task<IActionResult> Get(int rentalId, DateTime start, int nights, CancellationToken cancellationToken)
    {
        var calendarDatesResult = await _calendarService.GetCalendarDatesAsync(
            rentalId,
            DateOnly.FromDateTime(start),
            nights,
            cancellationToken);

        if (!calendarDatesResult.IsSuccess)
        {
            return BadRequest(new ErrorViewModel(calendarDatesResult.ErrorMessage));
        }

        if (calendarDatesResult.CalendarDates.Length == 0)
        {
            return NotFound();
        }

        return Ok(ViewModelMapper.MapRentalIdAndCalendarDatesToCalendarViewModel(rentalId, calendarDatesResult.CalendarDates));
    }
}