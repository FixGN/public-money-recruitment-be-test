using System.Diagnostics.CodeAnalysis;

namespace VacationRental.Api.Services.Models;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum CreateBookingResultErrorStatus
{
    Undefined = 0,
    ValidationFailed = 1,
    Conflict = 2
}