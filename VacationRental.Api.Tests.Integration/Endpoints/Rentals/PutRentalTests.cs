using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Contracts.Booking;
using VacationRental.Api.Contracts.Common;
using VacationRental.Api.Contracts.Rental;
using VacationRental.Api.Tests.Integration.Clients;
using VacationRental.Api.Tests.Integration.Infrastructure;
using Xunit;

namespace VacationRental.Api.Tests.Integration.Endpoints.Rentals;

[Collection("Integration")]
public class PutRentalTests
{
    private readonly HttpClient _client;
    private readonly RentalsClient _rentalsClient;
    private readonly BookingsClient _bookingsClient;

    private const int DefaultUnits = 25;
    private const int DefaultPreparationTimeInDays = 3;

    private readonly DateTime _defaultStartDate = new(2022, 1, 1);
    private const int DefaultNights = 2;

    public PutRentalTests(IntegrationFixture fixture)
    {
        _client = fixture.Client;
        _rentalsClient = fixture.RentalsClient;
        _bookingsClient = fixture.BookingsClient;
    }

    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsCorrectUpdatedRental()
    {
        var createRequest = new RentalBindingModel(DefaultUnits, DefaultPreparationTimeInDays);
        var createResponse = await _rentalsClient.CreateRentalAsync(createRequest);
        Assert.True(createResponse.IsSuccessStatusCode);

        var updateRequest = new RentalBindingModel(DefaultUnits + 1, DefaultPreparationTimeInDays + 2);
        var updateResponse = await _rentalsClient.UpdateRentalAsync(createResponse.Message!.Id, updateRequest);
        Assert.True(updateResponse.IsSuccessStatusCode);
            
        Assert.Equal(createResponse.Message!.Id, updateResponse.Message!.Id);
        Assert.Equal(updateRequest.Units, updateResponse.Message!.Units);
        Assert.Equal(updateRequest.PreparationTimeInDays, updateResponse.Message!.PreparationTimeInDays);
    }
    
    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsBadRequestIfUnitsIsNegative()
    {
        var createRequest = new RentalBindingModel(DefaultUnits, DefaultPreparationTimeInDays);
        var createResponse = await _rentalsClient.CreateRentalAsync(createRequest);
        Assert.True(createResponse.IsSuccessStatusCode);

        var updateRequest = new RentalBindingModel{Units = -1, PreparationTimeInDays = DefaultPreparationTimeInDays};
        var updateResponse = await _rentalsClient.UpdateRentalAsync(createResponse.Message!.Id, updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }
    
    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsBadRequestIfPreparationTimeInDaysIsNegative()
    {
        var createRequest = new RentalBindingModel(DefaultUnits, DefaultPreparationTimeInDays);
        var createResponse = await _rentalsClient.CreateRentalAsync(createRequest);
        Assert.True(createResponse.IsSuccessStatusCode);
        
        var updateRequest = new RentalBindingModel{Units = DefaultUnits, PreparationTimeInDays = -1};
        var updateResponse = await _rentalsClient.UpdateRentalAsync(createResponse.Message!.Id, updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }
    
    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsNotFoundIfRentalNotExists()
    {
        var updateRequest = new RentalBindingModel(DefaultUnits, DefaultPreparationTimeInDays);
        var updateResponse = await _rentalsClient.UpdateRentalAsync(99999, updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }
    
    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsConflictIfNewUnitsValueConflictsWithCreatedBookings()
    {
        var createRentalRequest = new RentalBindingModel(2, DefaultPreparationTimeInDays);
        var createRentalResponse = await _rentalsClient.CreateRentalAsync(createRentalRequest);
        Assert.True(createRentalResponse.IsSuccessStatusCode);

        var createBooking1Request = new BookingBindingModel(createRentalResponse.Message!.Id, _defaultStartDate, DefaultNights);
        var createBooking1Response = await _bookingsClient.CreateBookingAsync(createBooking1Request);
        Assert.True(createBooking1Response.IsSuccessStatusCode);
        
        var createBooking2Request = new BookingBindingModel(createRentalResponse.Message!.Id, _defaultStartDate, DefaultNights);
        var createBooking2Response = await _bookingsClient.CreateBookingAsync(createBooking2Request);
        Assert.True(createBooking2Response.IsSuccessStatusCode);

        var updateRentalRequest = new RentalBindingModel(1, DefaultPreparationTimeInDays);
        var updateRentalResponse = await _rentalsClient.UpdateRentalAsync(createRentalResponse.Message!.Id, updateRentalRequest);
        Assert.Equal(HttpStatusCode.Conflict, updateRentalResponse.StatusCode);
    }
    
    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsConflictIfNewPreparationTimeInDaysValueConflictsWithCreatedBookings()
    {
        var createRentalRequest = new RentalBindingModel(DefaultUnits, 1);
        var createRentalResponse = await _rentalsClient.CreateRentalAsync(createRentalRequest);
        Assert.True(createRentalResponse.IsSuccessStatusCode);

        var createBooking1Request = new BookingBindingModel(createRentalResponse.Message!.Id, _defaultStartDate, DefaultNights);
        var createBooking1Response = await _bookingsClient.CreateBookingAsync(createBooking1Request);
        Assert.True(createBooking1Response.IsSuccessStatusCode);
        
        var createBooking2Request = new BookingBindingModel(
            createRentalResponse.Message!.Id,
            _defaultStartDate.AddDays(DefaultNights + 1),
            DefaultNights);
        var createBooking2Response = await _bookingsClient.CreateBookingAsync(createBooking2Request);
        Assert.True(createBooking2Response.IsSuccessStatusCode);
        
        var updateRentalRequest = new RentalBindingModel(DefaultUnits, 3);
        var updateRentalResponse = await _rentalsClient.UpdateRentalAsync(createRentalResponse.Message!.Id, updateRentalRequest);
        Assert.Equal(HttpStatusCode.Conflict, updateRentalResponse.StatusCode);
    }
}