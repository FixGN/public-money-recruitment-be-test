using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Api.Services;
using VacationRental.Api.Contracts.Calendar;
using VacationRental.Api.Contracts.Common;

namespace VacationRental.Api.Controllers
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
            
            return Ok(MapCalendarViewModelFromRentalIdAndCalendarDates(rentalId, calendarDatesResult.CalendarDates));
        }
        
        private static CalendarViewModel MapCalendarViewModelFromRentalIdAndCalendarDates(int rentalId, CalendarDate[] calendarDates)
        {
            return new CalendarViewModel(
                rentalId,
                calendarDates.Select(MapCalendarDateToCalendarDateViewModel).ToList());
        }
        
        private static CalendarDateViewModel MapCalendarDateToCalendarDateViewModel(CalendarDate calendarDate)
        {
            return new CalendarDateViewModel(
                calendarDate.Date,
                calendarDate.Bookings.Select(x => new CalendarBookingViewModel(x.Id)).ToList());
        }
    }
}
