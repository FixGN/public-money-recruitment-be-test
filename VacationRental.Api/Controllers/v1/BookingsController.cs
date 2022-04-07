using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Contracts.Booking;
using VacationRental.Api.Contracts.Common;
using VacationRental.Api.Controllers.v1.Mappers;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Models;

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
            cancellationToken.ThrowIfCancellationRequested();
            
            var booking = await _bookingService.GetBookingOrDefaultAsync(bookingId, cancellationToken);

            return booking == null 
                ? NotFound()
                : Ok(ViewModelMapper.MapBookingToBookingViewModel(booking));
        }

        [HttpPost]
        public async Task<IActionResult> Post(BookingBindingModel model, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
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
                {ErrorStatus: CreateBookingResultErrorStatus.Conflict} 
                    => Conflict(new ErrorViewModel(bookingCreationResult.ErrorMessage)),
                _ => throw new ApplicationException("Unknown error status")
            };
        }
    }
}
