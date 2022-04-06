using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Contracts.Common;
using VacationRental.Api.Contracts.Rental;
using VacationRental.Api.Controllers.v1.Mappers;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Models;

namespace VacationRental.Api.Controllers.v1
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;
        
        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService ?? throw new ArgumentNullException(nameof(rentalService));
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public IActionResult Get(int rentalId)
        {
            var rental = _rentalService.GetRentalOrDefault(rentalId);

            return rental == null 
                ? NotFound() 
                : Ok(ViewModelMapper.MapRentalToRentalViewModel(rental));
        }

        [HttpPost]
        public IActionResult Post(RentalBindingModel model)
        {
            var rental = _rentalService.CreateRental(model.Units, model.PreparationTimeInDays);

            return Ok(new ResourceIdViewModel(rental.Id));
        }

        [HttpPut("{rentalId:int}")]
        public IActionResult Put(int rentalId, RentalBindingModel model)
        {
            var updateRentalResult = _rentalService.UpdateRental(rentalId, model.Units, model.PreparationTimeInDays);

            return updateRentalResult switch
            {
                {IsSuccess: true} => Ok(new RentalViewModel(rentalId, model.Units, model.PreparationTimeInDays)),
                {ErrorStatus: UpdateRentalErrorStatus.ValidationFailed} => BadRequest(new ErrorViewModel(updateRentalResult.ErrorMessage)),
                {ErrorStatus: UpdateRentalErrorStatus.RentalNotFound} => NotFound(new ErrorViewModel(updateRentalResult.ErrorMessage)),
                {ErrorStatus: UpdateRentalErrorStatus.Conflict} => Conflict(new ErrorViewModel(updateRentalResult.ErrorMessage)),
                _ => throw new ApplicationException("Unknown error status")
            };
        }
    }
}
