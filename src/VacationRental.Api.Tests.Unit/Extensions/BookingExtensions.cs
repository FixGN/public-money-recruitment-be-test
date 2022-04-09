using VacationRental.Api.Models;

namespace VacationRental.Api.Tests.Unit.Extensions;

internal static class BookingExtensions
{
    public static bool AreEqual(this Booking booking1, Booking booking2)
    {
        return booking1.Id == booking2.Id
               && booking1.RentalId == booking2.RentalId
               && booking1.Start == booking2.Start
               && booking1.Nights == booking2.Nights;
    }
}