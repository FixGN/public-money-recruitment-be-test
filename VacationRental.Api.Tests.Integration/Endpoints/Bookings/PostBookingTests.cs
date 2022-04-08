﻿using System;
using System.Net;
using System.Threading.Tasks;
using VacationRental.Api.Contracts.Booking;
using VacationRental.Api.Contracts.Rental;
using VacationRental.Api.Tests.Integration.Clients;
using VacationRental.Api.Tests.Integration.Infrastructure;
using Xunit;

namespace VacationRental.Api.Tests.Integration.Endpoints.Bookings
{
    [Collection("Integration")]
    public class PostBookingTests
    {
        private readonly BookingsClient _bookingsClient;
        private readonly RentalsClient _rentalsClient;

        public PostBookingTests(IntegrationFixture fixture)
        {
            _bookingsClient = fixture.BookingsClient;
            _rentalsClient = fixture.RentalsClient;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOnPreparation()
        {
            var createRentalRequest = new RentalBindingModel(2, 2);
            var createRentalResult = await _rentalsClient.CreateRentalAsync(createRentalRequest);
            Assert.True(createRentalResult.IsSuccess);

            var createBooking1Request = new BookingBindingModel(createRentalResult.Message!.Id, new DateTime(2001, 01, 01), 1);
            var createBooking1Result = await _bookingsClient.CreateBookingAsync(createBooking1Request);
            Assert.True(createBooking1Result.IsSuccess);
            
            var createBooking2Request = new BookingBindingModel(createRentalResult.Message!.Id, new DateTime(2001, 01, 02), 1);
            var createBooking2Result = await _bookingsClient.CreateBookingAsync(createBooking2Request);
            Assert.True(createBooking2Result.IsSuccess);
            
            var createBooking3Request = new BookingBindingModel(createRentalResult.Message!.Id, new DateTime(2001, 01, 03), 1);
            var createBooking3Result = await _bookingsClient.CreateBookingAsync(createBooking3Request);
            Assert.Equal(HttpStatusCode.Conflict, createBooking3Result.StatusCode);
        }
        
        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAGetReturnsTheCreatedBooking()
        {
            var createRentalRequest = new RentalBindingModel(4, 1);
            var createRentalResult = await _rentalsClient.CreateRentalAsync(createRentalRequest);
            Assert.True(createRentalResult.IsSuccess);

            var createBookingRequest = new BookingBindingModel(createRentalResult.Message!.Id, new DateTime(2001, 01, 01), 3);

            var createBookingResponse = await _bookingsClient.CreateBookingAsync(createBookingRequest);
            Assert.True(createBookingResponse.IsSuccess);

            var getBookingResponse = await _bookingsClient.GetBookingAsync(createBookingResponse.Message!.Id);
            Assert.True(getBookingResponse.IsSuccess);

            Assert.Equal(createBookingRequest.RentalId, getBookingResponse.Message!.RentalId);
            Assert.Equal(createBookingRequest.Nights, getBookingResponse.Message!.Nights);
            Assert.Equal(createBookingRequest.Start, getBookingResponse.Message!.Start);
            Assert.Equal(1, getBookingResponse.Message!.Unit);
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOverbooking()
        {
            var createRentalRequest = new RentalBindingModel(1, 1);
            var createRentalResponse = await _rentalsClient.CreateRentalAsync(createRentalRequest);
            Assert.True(createRentalResponse.IsSuccess);

            var createBooking1Request = new BookingBindingModel(createRentalResponse.Message!.Id, new DateTime(2002, 01, 01), 3);
            var createBooking1Response = await _bookingsClient.CreateBookingAsync(createBooking1Request);
            Assert.True(createBooking1Response.IsSuccess);

            var createBooking2Request = new BookingBindingModel(createRentalResponse.Message!.Id, new DateTime(2002, 01, 02), 1);
            var createBooking2Response = await _bookingsClient.CreateBookingAsync(createBooking2Request);
            Assert.Equal(HttpStatusCode.Conflict, createBooking2Response.StatusCode);

            // TODO: Can I change this test? I want to return 409 instead of 500
            // await Assert.ThrowsAsync<ApplicationException>(async () =>
            // {
            //     using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            //     {
            //     }
            // });
        }
    }
}
