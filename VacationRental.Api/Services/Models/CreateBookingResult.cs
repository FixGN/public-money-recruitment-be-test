using System.Diagnostics.CodeAnalysis;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services.Models;

public class CreateBookingResult
{
    private CreateBookingResult(bool isSuccess, Booking? createdBooking)
    {
        IsSuccess = isSuccess;
        CreatedBooking = createdBooking;
    }

    [MemberNotNullWhen(true, nameof(CreatedBooking))]
    public bool IsSuccess { get; }
    public Booking? CreatedBooking { get; }

    public static CreateBookingResult Fail()
    {
        return new CreateBookingResult(false, null);
    }
    
    public static CreateBookingResult Successful(Booking createdBooking)
    {
        return new CreateBookingResult(true, createdBooking);
    }
}