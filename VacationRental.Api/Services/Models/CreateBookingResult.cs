using System.Diagnostics.CodeAnalysis;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services.Models;

public class CreateBookingResult
{
    private CreateBookingResult(
        bool isSuccess,
        CreateBookingResultStatus status,
        string? errorMessage,
        Booking? createdBooking)
    {
        IsSuccess = isSuccess;
        Status = status;
        ErrorMessage = errorMessage;
        CreatedBooking = createdBooking;
    }
    
    [MemberNotNullWhen(true, nameof(CreatedBooking))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; }
    public CreateBookingResultStatus Status { get; }
    public string? ErrorMessage { get; }
    public Booking? CreatedBooking { get; }

    public static CreateBookingResult ValidationFail(string message)
    {
        return new CreateBookingResult(false, CreateBookingResultStatus.ValidationFailed, message, null);
    }

    public static CreateBookingResult Conflict(string message)
    {
        return new CreateBookingResult(false, CreateBookingResultStatus.Conflict, message, null);
    }
    
    public static CreateBookingResult Successful(Booking createdBooking)
    {
        return new CreateBookingResult(true, CreateBookingResultStatus.Success, null, createdBooking);
    }
}