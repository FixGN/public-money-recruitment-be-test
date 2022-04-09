using System.Diagnostics.CodeAnalysis;

namespace VacationRental.Api.Services.Models.Rental;

public class UpdateRentalResult
{
    private UpdateRentalResult(
        bool isSuccess,
        UpdateRentalResultErrorStatus resultErrorStatus,
        string? errorMessage,
        Api.Models.Rental? rental)
    {
        IsSuccess = isSuccess;
        ResultErrorStatus = resultErrorStatus;
        ErrorMessage = errorMessage;
        Rental = rental;
    }

    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    [MemberNotNullWhen(true, nameof(Rental))]
    public bool IsSuccess { get; }
    public UpdateRentalResultErrorStatus ResultErrorStatus { get; }
    public string? ErrorMessage { get; }
    public Api.Models.Rental? Rental { get; }
    
    public static UpdateRentalResult ValidationFailed(string message) 
        => new(false, UpdateRentalResultErrorStatus.ValidationFailed, message, null);

    public static UpdateRentalResult RentalNotFound(int id) 
        => new(false, UpdateRentalResultErrorStatus.RentalNotFound, $"Rental with id '{id}' not found", null);

    public static UpdateRentalResult Conflict(string message) 
        => new(false, UpdateRentalResultErrorStatus.Conflict, message, null);

    public static UpdateRentalResult Successful(Api.Models.Rental rental) 
        => new(true, UpdateRentalResultErrorStatus.Undefined, null, rental);
}