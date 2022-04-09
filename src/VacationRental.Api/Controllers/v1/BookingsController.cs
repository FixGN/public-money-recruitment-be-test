using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Contracts.v1.Booking;
using VacationRental.Api.Contracts.v1.Common;
using VacationRental.Api.Controllers.v1.Mappers;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Interfaces;
using VacationRental.Api.Services.Models;
using VacationRental.Api.Services.Models.Booking;

namespace VacationRental.Api.Controllers.v1
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
        public async Task<IActionResult> Get(int bookingId, CancellationToken cancellationToken)
        {
            var booking = await _bookingService.GetBookingOrDefaultAsync(bookingId, cancellationToken);

            return booking == null 
                ? NotFound()
                : Ok(ViewModelMapper.MapBookingToBookingViewModel(booking));
        }

        [HttpPost]
        public async Task<IActionResult> Post(BookingBindingModel model, CancellationToken cancellationToken)
        {
            var bookingCreationResult = await _bookingService.CreateBookingAsync(
                model.RentalId,
                model.Start,
                model.Nights,
                cancellationToken);

            return bookingCreationResult switch
            {
                {IsSuccess: true} 
                    => Ok(new ResourceIdViewModel(bookingCreationResult.CreatedBooking.Id)),
                {ErrorStatus: CreateBookingResultErrorStatus.ValidationFailed}
                    => BadRequest(new ErrorViewModel(bookingCreationResult.ErrorMessage)),
                // It's part of contract. I want to change this to HTTP Status Conflict (409) instead
                {ErrorStatus: CreateBookingResultErrorStatus.Conflict}
                    => throw new ApplicationException(bookingCreationResult.ErrorMessage),
                _ => throw new ApplicationException("Unknown error status")
            };
        }
    }
}
