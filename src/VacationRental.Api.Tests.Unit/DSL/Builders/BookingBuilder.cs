using System;
using VacationRental.Api.Models;

namespace VacationRental.Api.Tests.Unit.DSL.Builders;

internal class BookingBuilder
{
    private int _id = 1;
    private int _nights = 1;
    private int _rentalId = 1;
    private DateTime _startDate = new(2022, 1, 1);
    private int _unit = 1;

    public BookingBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public BookingBuilder WithRentalId(int rentalId)
    {
        _rentalId = rentalId;
        return this;
    }

    public BookingBuilder WithUnit(int unit)
    {
        _unit = unit;
        return this;
    }

    public BookingBuilder WithStartDate(DateTime startDate)
    {
        _startDate = startDate;
        return this;
    }

    public BookingBuilder WithNights(int nights)
    {
        _nights = nights;
        return this;
    }

    public Booking Please()
    {
        return new Booking(_id, _rentalId, _unit, _startDate, _nights);
    }
}