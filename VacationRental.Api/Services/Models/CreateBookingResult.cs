using System.Diagnostics.CodeAnalysis;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services.Models;

public class CreateBookingResult
{
    private CreateBookingResult(
        bool isSuccess,
        CreateBookingResultErrorStatus errorErrorErrorStatus,
        string? errorMessage,
        Booking? createdBooking)
    {
        IsSuccess = isSuccess;
        ErrorStatus = errorErrorErrorStatus;
        ErrorMessage = errorMessage;
        CreatedBooking = createdBooking;
    }
    
    [MemberNotNullWhen(true, nameof(CreatedBooking))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; }
    public CreateBookingResultErrorStatus ErrorStatus { get; }
    public string? ErrorMessage { get; }
    public Booking? CreatedBooking { get; }

    public static CreateBookingResult ValidationFail(string message)
    {
        return new CreateBookingResult(false, CreateBookingResultErrorStatus.ValidationFailed, message, null);
    }

    public static CreateBookingResult Conflict(string message)
    {
        return new CreateBookingResult(false, CreateBookingResultErrorStatus.Conflict, message, null);
    }
    
    public static CreateBookingResult Successful(Booking createdBooking)
    {
        return new CreateBookingResult(true, CreateBookingResultErrorStatus.Undefined, null, createdBooking);
    }
}