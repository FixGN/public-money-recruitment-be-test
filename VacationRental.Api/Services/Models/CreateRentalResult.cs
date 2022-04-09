using System.Diagnostics.CodeAnalysis;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services.Models;

public class CreateRentalResult
{
    private CreateRentalResult(bool isSuccess, CreateRentalResultErrorStatus errorStatus, string? errorMessage, Rental? rental)
    {
        IsSuccess = isSuccess;
        ErrorStatus = errorStatus;
        ErrorMessage = errorMessage;
        Rental = rental;
    }

    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    [MemberNotNullWhen(true, nameof(Rental))]
    public bool IsSuccess { get; }
    public CreateRentalResultErrorStatus ErrorStatus { get; }
    public string? ErrorMessage { get; }
    public Rental? Rental { get; }

    public static CreateRentalResult ValidationFail(string message) 
        => new(false, CreateRentalResultErrorStatus.ValidationFail, message, null);
    public static CreateRentalResult Successful(Rental rental) 
        => new(true, CreateRentalResultErrorStatus.Undefined, null, rental);
}