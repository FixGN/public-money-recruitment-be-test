using VacationRental.Api.Tests.Unit.DSL.Builders;

namespace VacationRental.Api.Tests.Unit.DSL;

internal static class Create
{
    public static BookingBuilder Booking()
    {
        return new BookingBuilder();
    }

    public static RentalBuilder Rental()
    {
        return new RentalBuilder();
    }
}