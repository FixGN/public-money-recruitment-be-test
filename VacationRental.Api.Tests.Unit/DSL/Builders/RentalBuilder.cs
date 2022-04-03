using VacationRental.Api.Models;

namespace VacationRental.Api.Tests.Unit.DSL.Builders;

internal class RentalBuilder
{
    private int _id = 1;
    private int _units = 1;
    
    public RentalBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public Rental Please()
    {
        return new(_id, _units);
    }
}