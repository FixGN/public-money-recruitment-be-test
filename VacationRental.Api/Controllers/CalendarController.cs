using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Services;
using VacationRental.Api.ViewModels;

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
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ApplicationException("Nights must be positive");
            // TODO: make readable NotFound errors
            // if (!_rentals.ContainsKey(rentalId))
            //     throw new ApplicationException("Rental not found");

            var calendarDates = _calendarService.GetCalendarDates(rentalId, start, nights);
            
            return new CalendarViewModel(rentalId, calendarDates.Select(CalendarDateViewModel.FromCalendarDate).ToList());
        }
    }
}
