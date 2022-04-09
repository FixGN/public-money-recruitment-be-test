using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Contracts.v1.Common;
using VacationRental.Api.Contracts.v1.Rental;
using VacationRental.Api.Controllers.v1.Mappers;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Interfaces;
using VacationRental.Api.Services.Models.Rental;

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
        public async Task<IActionResult> Get(int rentalId, CancellationToken cancellationToken)
        {
            var rental = await _rentalService.GetRentalOrDefaultAsync(rentalId, cancellationToken);

            return rental == null 
                ? NotFound() 
                : Ok(ViewModelMapper.MapRentalToRentalViewModel(rental));
        }

        [HttpPost]
        public async Task<IActionResult> Post(RentalBindingModel model, CancellationToken cancellationToken)
        {
            var createRentalResult = await _rentalService.CreateRentalAsync(model.Units, model.PreparationTimeInDays, cancellationToken);

            return createRentalResult switch
            {
                {IsSuccess: true} 
                    => Ok(new ResourceIdViewModel(createRentalResult.Rental.Id)),
                {ErrorStatus: CreateRentalResultErrorStatus.ValidationFailed} 
                    => BadRequest(new ErrorViewModel(createRentalResult.ErrorMessage)),
                _ => throw new ApplicationException("Unknown error status")
            };
        }

        [HttpPut("{rentalId:int}")]
        public async Task<IActionResult> Put(int rentalId, RentalBindingModel model)
        {
            var updateRentalResult = await _rentalService.UpdateRentalAsync(rentalId, model.Units, model.PreparationTimeInDays);

            return updateRentalResult switch
            {
                {IsSuccess: true} 
                    => Ok(new RentalViewModel(
                    updateRentalResult.Rental.Id,
                    updateRentalResult.Rental.Units,
                    updateRentalResult.Rental.PreparationTimeInDays)),
                {ResultErrorStatus: UpdateRentalResultErrorStatus.ValidationFailed}
                    => BadRequest(new ErrorViewModel(updateRentalResult.ErrorMessage)),
                {ResultErrorStatus: UpdateRentalResultErrorStatus.RentalNotFound} 
                    => NotFound(new ErrorViewModel(updateRentalResult.ErrorMessage)),
                {ResultErrorStatus: UpdateRentalResultErrorStatus.Conflict} 
                    => Conflict(new ErrorViewModel(updateRentalResult.ErrorMessage)),
                _ => throw new ApplicationException("Unknown error status")
            };
        }
    }
}
