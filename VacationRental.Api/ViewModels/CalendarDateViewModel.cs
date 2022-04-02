using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;

namespace VacationRental.Api.ViewModels
{
    public class CalendarDateViewModel
    {
        public DateTime Date { get; set; }
        public List<CalendarBookingViewModel>? Bookings { get; set; }
        
        public static CalendarDateViewModel FromCalendarDate(CalendarDate calendarDate)
        {
            return new CalendarDateViewModel
            {
                Bookings = calendarDate.Bookings.Select(CalendarBookingViewModel.FromBooking).ToList(),
                Date = calendarDate.Date
            };
        }
    }
}
