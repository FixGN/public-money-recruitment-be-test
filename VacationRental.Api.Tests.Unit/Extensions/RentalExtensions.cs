using VacationRental.Api.Models;

namespace VacationRental.Api.Tests.Unit.Extensions;

public static class RentalExtensions
{
    public static bool AreEqual(this Rental rental1, Rental rental2)
    {
        return rental1.Id == rental2.Id
               && rental1.Units == rental2.Units;
    }
}