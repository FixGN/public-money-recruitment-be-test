using System.Diagnostics.CodeAnalysis;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services.Models;

public class CreateBookingResult
{
    private CreateBookingResult(bool isSuccess, string? errorMessage, Booking? createdBooking)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        CreatedBooking = createdBooking;
    }

    [MemberNotNullWhen(true, nameof(CreatedBooking))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public Booking? CreatedBooking { get; }

    public static CreateBookingResult Fail(string message)
    {
        return new CreateBookingResult(false, message, null);
    }
    
    public static CreateBookingResult Successful(Booking createdBooking)
    {
        return new CreateBookingResult(true, null, createdBooking);
    }
}