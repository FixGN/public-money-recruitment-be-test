using System.Diagnostics.CodeAnalysis;

namespace VacationRental.Api.Services.Models.Rental;

public class CreateRentalResult
{
    private CreateRentalResult(bool isSuccess, CreateRentalResultErrorStatus errorStatus, string? errorMessage, Api.Models.Rental? rental)
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
    public Api.Models.Rental? Rental { get; }

    public static CreateRentalResult ValidationFail(string message) 
        => new(false, CreateRentalResultErrorStatus.ValidationFail, message, null);
    public static CreateRentalResult Successful(Api.Models.Rental rental) 
        => new(true, CreateRentalResultErrorStatus.Undefined, null, rental);
}