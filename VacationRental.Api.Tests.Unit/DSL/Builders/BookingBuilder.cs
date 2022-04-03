using System;
using VacationRental.Api.Models;

namespace VacationRental.Api.Tests.Unit.DSL.Builders;

internal class BookingBuilder
{
    private int _id = 1;
    private int _rentalId = 1;
    private DateTime _startDate = new(2022, 1, 1);
    private int _nights = 1;

    public BookingBuilder WithRentalId(int rentalId)
    {
        _rentalId = rentalId;
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
        return new(_id, _rentalId, _startDate, _nights);
    }
}