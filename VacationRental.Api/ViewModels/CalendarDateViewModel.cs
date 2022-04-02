using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;

namespace VacationRental.Api.ViewModels
{
    public class CalendarDateViewModel
    {
        public CalendarDateViewModel(DateTime date, List<CalendarBookingViewModel> bookings)
        {
            Date = date;
            Bookings = bookings ?? throw new ArgumentNullException(nameof(bookings));
        }
        
        public DateTime Date { get; }
        public List<CalendarBookingViewModel> Bookings { get; }
        
        public static CalendarDateViewModel FromCalendarDate(CalendarDate calendarDate)
        {
            return new CalendarDateViewModel(
                calendarDate.Date,
                calendarDate.Bookings.Select(CalendarBookingViewModel.FromBooking).ToList());
        }
    }
}
