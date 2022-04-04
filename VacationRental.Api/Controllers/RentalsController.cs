using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Services;
using VacationRental.Api.Contracts.Common;
using VacationRental.Api.Contracts.Rental;
using VacationRental.Api.Mappers;

namespace VacationRental.Api.Controllers
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
            var rental = _rentalService.CreateRental(model.Units);

            return Ok(new ResourceIdViewModel(rental.Id));
        }
    }
}
