using System.Diagnostics.CodeAnalysis;

namespace VacationRental.Api.Services.Models;

public class UpdateRentalResult
{
    private UpdateRentalResult(bool isSuccess, UpdateRentalErrorStatus errorStatus, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorStatus = errorStatus;
        ErrorMessage = errorMessage;
    }

    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; }
    public UpdateRentalErrorStatus ErrorStatus { get; }
    public string? ErrorMessage { get; }
    
    public static UpdateRentalResult ValidationFail(string message) 
        => new(false, UpdateRentalErrorStatus.ValidationFailed, message);

    public static UpdateRentalResult RentalNotFound(int id) 
        => new(false, UpdateRentalErrorStatus.RentalNotFound, $"Rental with id '{id}' not found");

    public static UpdateRentalResult Conflict(string message) 
        => new(false, UpdateRentalErrorStatus.Conflict, message);

    public static UpdateRentalResult Successful() 
        => new(true, UpdateRentalErrorStatus.Undefined, null);
}