using System.Diagnostics.CodeAnalysis;

namespace VacationRental.Api.Services.Models.Booking;

public class CreateBookingResult
{
    private CreateBookingResult(
        bool isSuccess,
        CreateBookingResultErrorStatus errorErrorErrorStatus,
        string? errorMessage,
        Api.Models.Booking? createdBooking)
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
    public Api.Models.Booking? CreatedBooking { get; }

    public static CreateBookingResult ValidationFailed(string message) 
        => new(false, CreateBookingResultErrorStatus.ValidationFailed, message, null);

    public static CreateBookingResult Conflict(string message) 
        => new(false, CreateBookingResultErrorStatus.Conflict, message, null);

    public static CreateBookingResult Successful(Api.Models.Booking createdBooking) 
        => new(true, CreateBookingResultErrorStatus.Undefined, null, createdBooking);
}