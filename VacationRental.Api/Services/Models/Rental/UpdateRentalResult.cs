using System.Diagnostics.CodeAnalysis;

namespace VacationRental.Api.Services.Models.Rental;

public class UpdateRentalResult
{
    private UpdateRentalResult(bool isSuccess, UpdateRentalResultErrorStatus resultErrorStatus, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ResultErrorStatus = resultErrorStatus;
        ErrorMessage = errorMessage;
    }

    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; }
    public UpdateRentalResultErrorStatus ResultErrorStatus { get; }
    public string? ErrorMessage { get; }
    
    public static UpdateRentalResult ValidationFailed(string message) 
        => new(false, UpdateRentalResultErrorStatus.ValidationFailed, message);

    public static UpdateRentalResult RentalNotFound(int id) 
        => new(false, UpdateRentalResultErrorStatus.RentalNotFound, $"Rental with id '{id}' not found");

    public static UpdateRentalResult Conflict(string message) 
        => new(false, UpdateRentalResultErrorStatus.Conflict, message);

    public static UpdateRentalResult Successful() 
        => new(true, UpdateRentalResultErrorStatus.Undefined, null);
}