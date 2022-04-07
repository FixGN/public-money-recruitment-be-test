using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Contracts.Booking;
using VacationRental.Api.Contracts.Common;
using VacationRental.Api.Contracts.Rental;
using Xunit;

namespace VacationRental.Api.Tests.Integration;

[Collection("Integration")]
public class PutRentalTests
{
    private readonly HttpClient _client;

    private const int DefaultUnits = 25;
    private const int DefaultPreparationTimeInDays = 3;

    private readonly DateTime _defaultStartDate = new(2022, 1, 1);
    private const int DefaultNights = 2;

    public PutRentalTests(IntegrationFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsCorrectUpdatedRental()
    {
        var postRequest = new RentalBindingModel(DefaultUnits, DefaultPreparationTimeInDays);

        ResourceIdViewModel postResult;
        using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRequest))
        {
            Assert.True(postResponse.IsSuccessStatusCode);
            postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }
        
        var putRequest = new RentalBindingModel(DefaultUnits + 1, DefaultPreparationTimeInDays + 2);

        using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", putRequest))
        {
            Assert.True(putResponse.IsSuccessStatusCode);
            var putResult = await putResponse.Content.ReadAsAsync<RentalViewModel>();
            
            Assert.Equal(postResult.Id, putResult.Id);
            Assert.Equal(putRequest.Units, putResult.Units);
            Assert.Equal(putRequest.PreparationTimeInDays, putResult.PreparationTimeInDays);
        }
    }
    
    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsBadRequestIfUnitsIsNegative()
    {
        var postRequest = new RentalBindingModel(DefaultUnits, DefaultPreparationTimeInDays);

        ResourceIdViewModel postResult;
        using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRequest))
        {
            Assert.True(postResponse.IsSuccessStatusCode);
            postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }
        
        var putRequest = new RentalBindingModel{Units = -1, PreparationTimeInDays = DefaultPreparationTimeInDays};

        using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", putRequest))
        {
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
        }
    }
    
    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsBadRequestIfPreparationTimeInDaysIsNegative()
    {
        var postRequest = new RentalBindingModel(DefaultUnits, DefaultPreparationTimeInDays);

        ResourceIdViewModel postResult;
        using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRequest))
        {
            Assert.True(postResponse.IsSuccessStatusCode);
            postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }
        
        var putRequest = new RentalBindingModel{Units = DefaultUnits, PreparationTimeInDays = -1};
        
        using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", putRequest))
        {
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
        }
    }
    
    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsNotFoundIfRentalNotExists()
    {
        var putRequest = new RentalBindingModel(DefaultUnits, DefaultPreparationTimeInDays);

        using var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{99999}", putRequest);
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
    }
    
    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsConflictIfNewUnitsValueConflictsWithCreatedBookings()
    {
        var rentalPostRequest = new RentalBindingModel(2, DefaultPreparationTimeInDays);

        ResourceIdViewModel rentalPostResult;
        using (var rentalPostResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", rentalPostRequest))
        {
            Assert.True(rentalPostResponse.IsSuccessStatusCode);
            rentalPostResult = await rentalPostResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }

        var postBooking1Request = new BookingBindingModel(rentalPostResult.Id, _defaultStartDate, DefaultNights);
        
        ResourceIdViewModel postBooking1Result;
        using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
        {
            Assert.True(postBooking1Response.IsSuccessStatusCode);
            postBooking1Result = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
        }
        
        var postBooking2Request = new BookingBindingModel(rentalPostResult.Id, _defaultStartDate, DefaultNights);
        
        ResourceIdViewModel postBooking2Result;
        using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
        {
            Assert.True(postBooking2Response.IsSuccessStatusCode);
            postBooking2Result = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
        }
        
        var putRequest = new RentalBindingModel(1, DefaultPreparationTimeInDays);

        using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{rentalPostResult.Id}", putRequest))
        {
            Assert.Equal(HttpStatusCode.Conflict, putResponse.StatusCode);
        }
    }
    
    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenReturnsConflictIfNewPreparationTimeInDaysValueConflictsWithCreatedBookings()
    {
        var rentalPostRequest = new RentalBindingModel(DefaultUnits, 1);

        ResourceIdViewModel rentalPostResult;
        using (var rentalPostResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", rentalPostRequest))
        {
            Assert.True(rentalPostResponse.IsSuccessStatusCode);
            rentalPostResult = await rentalPostResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }

        var postBooking1Request = new BookingBindingModel(rentalPostResult.Id, _defaultStartDate, DefaultNights);
        
        ResourceIdViewModel postBooking1Result;
        using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
        {
            Assert.True(postBooking1Response.IsSuccessStatusCode);
            postBooking1Result = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
        }
        
        var postBooking2Request = new BookingBindingModel(rentalPostResult.Id, _defaultStartDate.AddDays(DefaultNights + 1), DefaultNights);
        
        ResourceIdViewModel postBooking2Result;
        using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
        {
            Assert.True(postBooking2Response.IsSuccessStatusCode);
            postBooking2Result = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
        }
        
        var putRequest = new RentalBindingModel(DefaultUnits, 3);

        using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{rentalPostResult.Id}", putRequest))
        {
            Assert.Equal(HttpStatusCode.Conflict, putResponse.StatusCode);
        }
    }
}