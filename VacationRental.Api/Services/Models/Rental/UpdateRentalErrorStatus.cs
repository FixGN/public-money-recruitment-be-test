namespace VacationRental.Api.Services.Models.Rental;

public enum UpdateRentalErrorStatus
{
    Undefined = 0,
    ValidationFailed = 1,
    RentalNotFound = 2,
    Conflict = 3
}