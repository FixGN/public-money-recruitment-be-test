﻿using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Models;
using VacationRental.Api.Contracts.Booking;
using VacationRental.Api.Contracts.Common;
using VacationRental.Api.Mappers;

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
            var booking = _bookingService.GetBookingOrDefault(bookingId);

            return booking == null 
                ? NotFound()
                : Ok(ViewModelMapper.MapBookingToBookingViewModel(booking));
        }

        [HttpPost]
        public IActionResult Post(BookingBindingModel model)
        {
            var bookingCreationResult = _bookingService.CreateBooking(model.RentalId, model.Start, model.Nights);

            return bookingCreationResult switch
            {
                {IsSuccess: true} => Ok(new ResourceIdViewModel(bookingCreationResult.CreatedBooking.Id)),
                {Status: CreateBookingResultStatus.ValidationFailed} => BadRequest(new ErrorViewModel(bookingCreationResult.ErrorMessage)),
                {Status: CreateBookingResultStatus.Conflict} => Conflict(new ErrorViewModel(bookingCreationResult.ErrorMessage)),
                _ => throw new ApplicationException("Unknown error status")
            };
        }
    }
}
