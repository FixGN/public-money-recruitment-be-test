using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Api.Services;
using VacationRental.Api.Contracts.Booking;
using VacationRental.Api.Contracts.Common;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public IActionResult Get(int bookingId)
        {
            var booking = _bookingService.GetBooking(bookingId);

            return booking == null 
                ? NotFound()
                : Ok(MapBookingToBookingViewModel(booking));
        }

        [HttpPost]
        public IActionResult Post(BookingBindingModel model)
        {
            var bookingCreationResult = _bookingService.CreateBooking(model.RentalId, model.Start, model.Nights);

            return bookingCreationResult.IsSuccess
                ? Ok(new ResourceIdViewModel(bookingCreationResult.CreatedBooking.Id))
                : BadRequest(bookingCreationResult.ErrorMessage);
        }

        private static BookingViewModel MapBookingToBookingViewModel(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking));
            }
            
            return new BookingViewModel(booking.Id, booking.RentalId, booking.Start, booking.Nights);
        }
    }
}
