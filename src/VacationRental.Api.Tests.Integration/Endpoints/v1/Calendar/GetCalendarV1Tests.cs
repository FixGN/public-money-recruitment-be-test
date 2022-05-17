using System;
using System.Threading.Tasks;
using VacationRental.Api.Contracts.v1.Booking;
using VacationRental.Api.Contracts.v1.Rental;
using VacationRental.Api.Tests.Integration.Clients.v1;
using VacationRental.Api.Tests.Integration.Infrastructure;
using Xunit;

namespace VacationRental.Api.Tests.Integration.Endpoints.v1.Calendar;

[Collection("Integration")]
public class GetCalendarTests
{
    private readonly BookingsV1Client _bookingsV1Client;
    private readonly CalendarV1Client _calendarV1Client;
    private readonly RentalsV1Client _rentalsV1Client;

    public GetCalendarTests(IntegrationFixture fixture)
    {
        _bookingsV1Client = fixture.BookingsV1Client;
        _rentalsV1Client = fixture.RentalsV1Client;
        _calendarV1Client = fixture.CalendarV1Client;
    }

    [Fact]
    public async Task GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendarWithPreparationTimesInFirstDays()
    {
        var createRentalRequest = new RentalBindingModel(2, 2);
        var createRentalResponse = await _rentalsV1Client.CreateRentalAsync(createRentalRequest);
        Assert.True(createRentalResponse.IsSuccessStatusCode);

        var createBooking1Request = new BookingBindingModel(
            createRentalResponse.Message!.Id,
            new DateTime(2000, 01, 01),
            1);
        var createBooking1Response = await _bookingsV1Client.CreateBookingAsync(createBooking1Request);
        Assert.True(createBooking1Response.IsSuccessStatusCode);

        var getBooking1Response = await _bookingsV1Client.GetBookingAsync(createBooking1Response.Message!.Id);
        Assert.True(getBooking1Response.IsSuccessStatusCode);

        var createBooking2Request = new BookingBindingModel(
            createRentalResponse.Message!.Id,
            new DateTime(2000, 01, 02),
            1);
        var createBooking2Response = await _bookingsV1Client.CreateBookingAsync(createBooking2Request);
        Assert.True(createBooking2Response.IsSuccessStatusCode);

        var getBooking2Response = await _bookingsV1Client.GetBookingAsync(createBooking2Response.Message!.Id);
        Assert.True(getBooking2Response.IsSuccessStatusCode);

        var createBooking3Request = new BookingBindingModel(
            createRentalResponse.Message!.Id,
            new DateTime(2000, 01, 04),
            2);
        var createBooking3Response = await _bookingsV1Client.CreateBookingAsync(createBooking3Request);
        Assert.True(createBooking3Response.IsSuccessStatusCode);

        var getBooking3Response = await _bookingsV1Client.GetBookingAsync(createBooking3Response.Message!.Id);
        Assert.True(getBooking3Response.IsSuccessStatusCode);

        var getCalendarResponse = await _calendarV1Client.GetCalendarAsync(
            createRentalResponse.Message!.Id,
            new DateTime(2000, 01, 03),
            2);
        Assert.True(getCalendarResponse.IsSuccessStatusCode);

        // Assert
        // Common
        Assert.Equal(createRentalResponse.Message!.Id, getCalendarResponse.Message!.RentalId);
        Assert.Equal(2, getCalendarResponse.Message!.Dates.Count);

        // First day
        Assert.Equal(new DateTime(2000, 01, 03), getCalendarResponse.Message!.Dates[0].Date);
        Assert.Empty(getCalendarResponse.Message!.Dates[0].Bookings);
        Assert.Equal(2, getCalendarResponse.Message!.Dates[0].PreparationTimes.Count);
        Assert.Contains(
            getCalendarResponse.Message!.Dates[0].PreparationTimes,
            x => x.Unit == getBooking1Response.Message!.Unit);
        Assert.Contains(
            getCalendarResponse.Message!.Dates[0].PreparationTimes,
            x => x.Unit == getBooking2Response.Message!.Unit);

        // Second day
        Assert.Equal(new DateTime(2000, 01, 04), getCalendarResponse.Message!.Dates[1].Date);
        Assert.Single(getCalendarResponse.Message!.Dates[1].Bookings);
        Assert.Contains(
            getCalendarResponse.Message!.Dates[1].Bookings,
            x => x.Id == getBooking3Response.Message!.Id && x.Unit == getBooking3Response.Message!.Unit);
        Assert.Single(getCalendarResponse.Message!.Dates[1].PreparationTimes);
        Assert.Contains(
            getCalendarResponse.Message!.Dates[1].PreparationTimes,
            x => x.Unit == getBooking2Response.Message!.Unit);
    }

    [Fact]
    public async Task GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendar()
    {
        var createRentalRequest = new RentalBindingModel(2, 1);
        var createRentalResponse = await _rentalsV1Client.CreateRentalAsync(createRentalRequest);
        Assert.True(createRentalResponse.IsSuccessStatusCode);

        var createBooking1Request = new BookingBindingModel(
            createRentalResponse.Message!.Id,
            new DateTime(2000, 01, 02),
            2);
        var createBooking1Response = await _bookingsV1Client.CreateBookingAsync(createBooking1Request);
        Assert.True(createBooking1Response.IsSuccessStatusCode);

        var getBooking1Response = await _bookingsV1Client.GetBookingAsync(createBooking1Response.Message!.Id);
        Assert.True(getBooking1Response.IsSuccessStatusCode);

        var createBooking2Request = new BookingBindingModel(
            createRentalResponse.Message!.Id,
            new DateTime(2000, 01, 03),
            2);
        var createBooking2Response = await _bookingsV1Client.CreateBookingAsync(createBooking2Request);
        Assert.True(createBooking2Response.IsSuccessStatusCode);

        var getBooking2Response = await _bookingsV1Client.GetBookingAsync(createBooking2Response.Message!.Id);
        Assert.True(getBooking2Response.IsSuccessStatusCode);

        var getCalendarResponse = await _calendarV1Client.GetCalendarAsync(
            createRentalResponse.Message!.Id,
            new DateTime(2000, 01, 01),
            6);
        Assert.True(getCalendarResponse.IsSuccessStatusCode);

        // Assert
        // Common
        Assert.Equal(createRentalResponse.Message!.Id, getCalendarResponse.Message!.RentalId);
        Assert.Equal(6, getCalendarResponse.Message!.Dates.Count);

        // First day
        Assert.Equal(new DateTime(2000, 01, 01), getCalendarResponse.Message!.Dates[0].Date);
        Assert.Empty(getCalendarResponse.Message!.Dates[0].Bookings);
        Assert.Empty(getCalendarResponse.Message!.Dates[0].PreparationTimes);

        // Second day
        Assert.Equal(new DateTime(2000, 01, 02), getCalendarResponse.Message!.Dates[1].Date);
        Assert.Single(getCalendarResponse.Message!.Dates[1].Bookings);
        Assert.Contains(
            getCalendarResponse.Message!.Dates[1].Bookings,
            x => x.Id == getBooking1Response.Message!.Id && x.Unit == getBooking1Response.Message!.Unit);
        Assert.Empty(getCalendarResponse.Message!.Dates[1].PreparationTimes);

        // Third day
        Assert.Equal(new DateTime(2000, 01, 03), getCalendarResponse.Message!.Dates[2].Date);
        Assert.Equal(2, getCalendarResponse.Message!.Dates[2].Bookings.Count);
        Assert.Contains(
            getCalendarResponse.Message!.Dates[2].Bookings,
            x => x.Id == getBooking1Response.Message!.Id && x.Unit == getBooking1Response.Message!.Unit);
        Assert.Contains(
            getCalendarResponse.Message!.Dates[2].Bookings,
            x => x.Id == getBooking2Response.Message!.Id && x.Unit == getBooking2Response.Message!.Unit);
        Assert.Empty(getCalendarResponse.Message!.Dates[2].PreparationTimes);

        // Forth day
        Assert.Equal(new DateTime(2000, 01, 04), getCalendarResponse.Message!.Dates[3].Date);
        Assert.Single(getCalendarResponse.Message!.Dates[3].Bookings);
        Assert.Contains(
            getCalendarResponse.Message!.Dates[3].Bookings,
            x => x.Id == getBooking2Response.Message!.Id && x.Unit == getBooking2Response.Message!.Unit);
        Assert.Single(getCalendarResponse.Message!.Dates[3].PreparationTimes);
        Assert.Contains(
            getCalendarResponse.Message!.Dates[3].PreparationTimes,
            x => x.Unit == getBooking1Response.Message!.Unit);

        // Fifth day
        Assert.Equal(new DateTime(2000, 01, 05), getCalendarResponse.Message!.Dates[4].Date);
        Assert.Empty(getCalendarResponse.Message!.Dates[4].Bookings);
        Assert.Single(getCalendarResponse.Message!.Dates[4].PreparationTimes);
        Assert.Contains(
            getCalendarResponse.Message!.Dates[4].PreparationTimes,
            x => x.Unit == getBooking2Response.Message!.Unit);

        // Sixth day
        Assert.Equal(new DateTime(2000, 01, 06), getCalendarResponse.Message!.Dates[5].Date);
        Assert.Empty(getCalendarResponse.Message!.Dates[5].Bookings);
        Assert.Empty(getCalendarResponse.Message!.Dates[5].PreparationTimes);
    }
}