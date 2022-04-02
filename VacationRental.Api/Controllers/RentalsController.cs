using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Api.Services;
using VacationRental.Contracts.Common;
using VacationRental.Contracts.Rental;

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
            var rental = _rentalService.Get(rentalId);

            return rental == null 
                ? NotFound() 
                : Ok(MapRentalToRentalViewModel(rental));;
        }

        [HttpPost]
        public IActionResult Post(RentalBindingModel model)
        {
            var rental = _rentalService.Create(model.Units);

            return Ok(new ResourceIdViewModel(rental.Id));
        }
        
        private static RentalViewModel MapRentalToRentalViewModel(Rental rental)
        {
            return new RentalViewModel(rental.Id, rental.Units);
        }
    }
}
