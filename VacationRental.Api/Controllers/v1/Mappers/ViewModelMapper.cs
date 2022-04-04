using System;
using System.Linq;
using VacationRental.Api.Contracts.Booking;
using VacationRental.Api.Contracts.Calendar;
using VacationRental.Api.Contracts.Rental;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers.v1.Mappers;

public static class ViewModelMapper
{
    public static BookingViewModel MapBookingToBookingViewModel(Booking booking)
    {
        if (booking == null)
        {
            throw new ArgumentNullException(nameof(booking));
        }
            
        return new BookingViewModel(booking.Id, booking.RentalId, booking.Start, booking.Nights);
    }
    
    public static CalendarViewModel MapRentalIdAndCalendarDatesToCalendarViewModel(int rentalId, CalendarDate[] calendarDates)
    {
        return new CalendarViewModel(
            rentalId,
            calendarDates.Select(MapCalendarDatesToCalendarDatesViewModel).ToList());
    }
    
    public static RentalViewModel MapRentalToRentalViewModel(Rental rental)
    {
        return new RentalViewModel(rental.Id, rental.Units);
    }
        
    private static CalendarDateViewModel MapCalendarDatesToCalendarDatesViewModel(CalendarDate calendarDate)
    {
        return new CalendarDateViewModel(
            calendarDate.Date,
            calendarDate.Bookings.Select(x => new CalendarBookingViewModel(x.Id)).ToList());
    }
}