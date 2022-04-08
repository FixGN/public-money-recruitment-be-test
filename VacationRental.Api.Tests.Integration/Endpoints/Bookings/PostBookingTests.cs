using System;
using System.Net;
using System.Threading.Tasks;
using VacationRental.Api.Contracts.Booking;
using VacationRental.Api.Contracts.Rental;
using VacationRental.Api.Tests.Integration.Clients;
using VacationRental.Api.Tests.Integration.Clients.v1;
using VacationRental.Api.Tests.Integration.Infrastructure;
using Xunit;

namespace VacationRental.Api.Tests.Integration.Endpoints.Bookings
{
    [Collection("Integration")]
    public class PostBookingTests
    {
        private readonly BookingsV1Client _bookingsV1Client;
        private readonly RentalsV1Client _rentalsV1Client;

        public PostBookingTests(IntegrationFixture fixture)
        {
            _bookingsV1Client = fixture.BookingsV1Client;
            _rentalsV1Client = fixture.RentalsV1Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOnPreparation()
        {
            var createRentalRequest = new RentalBindingModel(2, 2);
            var createRentalResult = await _rentalsV1Client.CreateRentalAsync(createRentalRequest);
            Assert.True(createRentalResult.IsSuccessStatusCode);

            var createBooking1Request = new BookingBindingModel(createRentalResult.Message!.Id, new DateTime(2001, 01, 01), 1);
            var createBooking1Result = await _bookingsV1Client.CreateBookingAsync(createBooking1Request);
            Assert.True(createBooking1Result.IsSuccessStatusCode);
            
            var createBooking2Request = new BookingBindingModel(createRentalResult.Message!.Id, new DateTime(2001, 01, 02), 1);
            var createBooking2Result = await _bookingsV1Client.CreateBookingAsync(createBooking2Request);
            Assert.True(createBooking2Result.IsSuccessStatusCode);
            
            var createBooking3Request = new BookingBindingModel(createRentalResult.Message!.Id, new DateTime(2001, 01, 03), 1);
            var createBooking3Result = await _bookingsV1Client.CreateBookingAsync(createBooking3Request);
            Assert.Equal(HttpStatusCode.Conflict, createBooking3Result.StatusCode);
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

            var createBooking1Request = new BookingBindingModel(createRentalResponse.Message!.Id, new DateTime(2002, 01, 01), 3);
            var createBooking1Response = await _bookingsV1Client.CreateBookingAsync(createBooking1Request);
            Assert.True(createBooking1Response.IsSuccessStatusCode);

            var createBooking2Request = new BookingBindingModel(createRentalResponse.Message!.Id, new DateTime(2002, 01, 02), 1);
            var createBooking2Response = await _bookingsV1Client.CreateBookingAsync(createBooking2Request);
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
