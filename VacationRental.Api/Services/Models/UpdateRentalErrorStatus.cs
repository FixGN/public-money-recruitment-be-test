namespace VacationRental.Api.Services.Models;

public enum UpdateRentalErrorStatus
{
    Undefined = 0,
    ValidationError = 1,
    RentalNotFound = 2,
    Conflict = 3
}