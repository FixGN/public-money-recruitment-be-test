using System.Diagnostics.CodeAnalysis;

namespace VacationRental.Api.Services.Models;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum CreateBookingResultStatus
{
    Undefined = 0,
    Success = 1,
    ValidationFailed = 2,
    Conflict = 3
}