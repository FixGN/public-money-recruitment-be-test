﻿using System;
using System.Threading.Tasks;
using VacationRental.Api.Contracts.v1.Booking;
using VacationRental.Api.Contracts.v1.Rental;
using VacationRental.Api.Tests.Integration.Clients.v1;
using VacationRental.Api.Tests.Integration.Infrastructure;
using Xunit;

namespace VacationRental.Api.Tests.Integration.Endpoints.v1.Bookings
{
    [Collection("Integration")]
    public class PostBookingV1Tests
    {
        private readonly BookingsV1Client _bookingsV1Client;
        private readonly RentalsV1Client _rentalsV1Client;

        public PostBookingV1Tests(IntegrationFixture fixture)
        {
            _bookingsV1Client = fixture.BookingsV1Client;
            _rentalsV1Client = fixture.RentalsV1Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenAvailableUnitOnPreparation()
        {
            var createRentalRequest = new RentalBindingModel(2, 2);
            var createRentalResponse = await _rentalsV1Client.CreateRentalAsync(createRentalRequest);
            Assert.True(createRentalResponse.IsSuccessStatusCode);

            var createBooking1Request = new BookingBindingModel(
                createRentalResponse.Message!.Id,
                new DateTime(2001, 01, 01),
                1);
            var createBooking1Response = await _bookingsV1Client.CreateBookingAsync(createBooking1Request);
            Assert.True(createBooking1Response.IsSuccessStatusCode);
            
            var createBooking2Request = new BookingBindingModel(
                createRentalResponse.Message!.Id,
                new DateTime(2001, 01, 02),
                1);
            var createBooking2Response = await _bookingsV1Client.CreateBookingAsync(createBooking2Request);
            Assert.True(createBooking2Response.IsSuccessStatusCode);
            
            var createBooking3Request = new BookingBindingModel(
                createRentalResponse.Message!.Id,
                new DateTime(2001, 01, 03),
                1);
            // It's part of contracts. I want making HTTP status Conflict (409) instead 
            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                _ = await _bookingsV1Client.CreateBookingAsync(createBooking3Request);
            });
        }
        
        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAGetReturnsTheCreatedBooking()
        {
            var createRentalRequest = new RentalBindingModel(4, 1);
            var createRentalResult = await _rentalsV1Client.CreateRentalAsync(createRentalRequest);
            Assert.True(createRentalResult.IsSuccessStatusCode);

            var createBookingRequest = new BookingBindingModel(createRentalResult.Message!.Id, new DateTime(2001, 01, 01), 3);

            var createBookingResponse = await _bookingsV1Client.CreateBookingAsync(createBookingRequest);
            Assert.True(createBookingResponse.IsSuccessStatusCode);

            var getBookingResponse = await _bookingsV1Client.GetBookingAsync(createBookingResponse.Message!.Id);
            Assert.True(getBookingResponse.IsSuccessStatusCode);

            Assert.Equal(createBookingRequest.RentalId, getBookingResponse.Message!.RentalId);
            Assert.Equal(createBookingRequest.Nights, getBookingResponse.Message!.Nights);
            Assert.Equal(createBookingRequest.Start, getBookingResponse.Message!.Start);
            Assert.Equal(1, getBookingResponse.Message!.Unit);
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOverbooking()
        {
            var createRentalRequest = new RentalBindingModel(1, 1);
            var createRentalResponse = await _rentalsV1Client.CreateRentalAsync(createRentalRequest);
            Assert.True(createRentalResponse.IsSuccessStatusCode);

            var createBooking1Request = new BookingBindingModel(
                createRentalResponse.Message!.Id,
                new DateTime(2002, 01, 01),
                3);
            var createBooking1Response = await _bookingsV1Client.CreateBookingAsync(createBooking1Request);
            Assert.True(createBooking1Response.IsSuccessStatusCode);

            var createBooking2Request = new BookingBindingModel(
                createRentalResponse.Message!.Id,
                new DateTime(2002, 01, 02),
                1);
            // It's part of contracts. I want making HTTP status Conflict (409) instead 
            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                _ = await _bookingsV1Client.CreateBookingAsync(createBooking2Request);
            });
        }
    }
}
