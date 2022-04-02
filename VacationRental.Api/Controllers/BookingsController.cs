﻿using System;
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

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(MapBookingToBookingViewModel(booking));
        }

        [HttpPost]
        public IActionResult Post(BookingBindingModel model)
        {
            if (model.Nights <= 0)
                throw new ApplicationException("Nights must be positive");
            // TODO: make readable badRequest
            // if (!_rentals.ContainsKey(model.RentalId))
            //     throw new ApplicationException("Rental not found");

            var bookingCreationResult = _bookingService.CreateBooking(model.RentalId, model.Start, model.Nights);

            if (!bookingCreationResult.IsSuccess)
            {
                return BadRequest();
            }

            return Ok(new ResourceIdViewModel(bookingCreationResult.CreatedBooking.Id));
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
