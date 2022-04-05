using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Contracts.Common;
using VacationRental.Api.Controllers.v1.Mappers;
using VacationRental.Api.Models;
using VacationRental.Api.Services;

namespace VacationRental.Api.Controllers.v1
{
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
        public IActionResult Get(int rentalId, DateTime start, int nights)
        {
            var calendarDatesResult = _calendarService.GetCalendarDates(rentalId, start, nights);

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
}
