using System;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Contracts.Booking;
using VacationRental.Api.Contracts.Calendar;
using VacationRental.Api.Contracts.Common;
using VacationRental.Api.Contracts.Rental;
using Xunit;

namespace VacationRental.Api.Tests.Integration
{
    [Collection("Integration")]
    public class GetCalendarTests
    {
        private readonly HttpClient _client;

        public GetCalendarTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendarWithPreparationTimesInFirstDays()
        {
            var postRentalRequest = new RentalBindingModel(2, 2);

            ResourceIdViewModel postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }
            
            var postBooking1Request = new BookingBindingModel(postRentalResult.Id,  new DateTime(2000, 01, 01), 1);

            ResourceIdViewModel postBooking1Result;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }
            
            BookingViewModel getBooking1Result;
            using (var getBooking1Response = await _client.GetAsync($"/api/v1/bookings/{postBooking1Result.Id}"))
            {
                Assert.True(getBooking1Response.IsSuccessStatusCode);
                getBooking1Result = await getBooking1Response.Content.ReadAsAsync<BookingViewModel>();
            }
            
            var postBooking2Request = new BookingBindingModel(postRentalResult.Id,  new DateTime(2000, 01, 02), 1);

            ResourceIdViewModel postBooking2Result;
            using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                Assert.True(postBooking2Response.IsSuccessStatusCode);
                postBooking2Result = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }
            
            BookingViewModel getBooking2Result;
            using (var getBooking2Response = await _client.GetAsync($"/api/v1/bookings/{postBooking2Result.Id}"))
            {
                Assert.True(getBooking2Response.IsSuccessStatusCode);
                getBooking2Result = await getBooking2Response.Content.ReadAsAsync<BookingViewModel>();
            }
            
            var postBooking3Request = new BookingBindingModel(postRentalResult.Id,  new DateTime(2000, 01, 04), 2);

            ResourceIdViewModel postBooking3Result;
            using (var postBooking3Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking3Request))
            {
                Assert.True(postBooking3Response.IsSuccessStatusCode);
                postBooking3Result = await postBooking3Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }
            
            BookingViewModel getBooking3Result;
            using (var getBooking3Response = await _client.GetAsync($"/api/v1/bookings/{postBooking3Result.Id}"))
            {
                Assert.True(getBooking3Response.IsSuccessStatusCode);
                getBooking3Result = await getBooking3Response.Content.ReadAsAsync<BookingViewModel>();
            }
            
            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start=2000-01-03&nights=2"))
            {
                Assert.True(getCalendarResponse.IsSuccessStatusCode);

                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();
                
                Assert.Equal(postRentalResult.Id, getCalendarResult.RentalId);
                Assert.Equal(2, getCalendarResult.Dates.Count);

                Assert.Equal(new DateTime(2000, 01, 03), getCalendarResult.Dates[0].Date);
                Assert.Empty(getCalendarResult.Dates[0].Bookings);
                Assert.Equal(2, getCalendarResult.Dates[0].PreparationTimes.Count);
                Assert.Contains(
                    getCalendarResult.Dates[0].PreparationTimes,
                    x => x.Unit == getBooking1Result.Unit);
                Assert.Contains(
                    getCalendarResult.Dates[0].PreparationTimes,
                    x => x.Unit == getBooking2Result.Unit);
                
                Assert.Equal(new DateTime(2000, 01, 04), getCalendarResult.Dates[1].Date);
                Assert.Single(getCalendarResult.Dates[1].Bookings);
                Assert.Contains(
                    getCalendarResult.Dates[1].Bookings,
                    x => x.Id == postBooking3Result.Id && x.Unit == getBooking3Result.Unit);
                Assert.Single(getCalendarResult.Dates[1].PreparationTimes);
                Assert.Contains(
                    getCalendarResult.Dates[1].PreparationTimes,
                    x => x.Unit == getBooking2Result.Unit);
            }
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendar()
        {
            var postRentalRequest = new RentalBindingModel(2, 1);

            ResourceIdViewModel postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking1Request = new BookingBindingModel(postRentalResult.Id,  new DateTime(2000, 01, 02), 2);

            ResourceIdViewModel postBooking1Result;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            BookingViewModel getBooking1Result;
            using (var getBooking1Response = await _client.GetAsync($"/api/v1/bookings/{postBooking1Result.Id}"))
            {
                Assert.True(getBooking1Response.IsSuccessStatusCode);
                getBooking1Result = await getBooking1Response.Content.ReadAsAsync<BookingViewModel>();
            }

            var postBooking2Request = new BookingBindingModel(postRentalResult.Id,  new DateTime(2000, 01, 03), 2);

            ResourceIdViewModel postBooking2Result;
            using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                Assert.True(postBooking2Response.IsSuccessStatusCode);
                postBooking2Result = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }
            
            BookingViewModel getBooking2Result;
            using (var getBooking2Response = await _client.GetAsync($"/api/v1/bookings/{postBooking2Result.Id}"))
            {
                Assert.True(getBooking2Response.IsSuccessStatusCode);
                getBooking2Result = await getBooking2Response.Content.ReadAsAsync<BookingViewModel>();
            }

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start=2000-01-01&nights=6"))
            {
                Assert.True(getCalendarResponse.IsSuccessStatusCode);

                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();
                
                Assert.Equal(postRentalResult.Id, getCalendarResult.RentalId);
                Assert.Equal(6, getCalendarResult.Dates.Count);

                Assert.Equal(new DateTime(2000, 01, 01), getCalendarResult.Dates[0].Date);
                Assert.Empty(getCalendarResult.Dates[0].Bookings);
                Assert.Empty(getCalendarResult.Dates[0].PreparationTimes);
                
                Assert.Equal(new DateTime(2000, 01, 02), getCalendarResult.Dates[1].Date);
                Assert.Single(getCalendarResult.Dates[1].Bookings);
                Assert.Contains(
                    getCalendarResult.Dates[1].Bookings,
                    x => x.Id == postBooking1Result.Id && x.Unit == getBooking1Result.Unit);
                Assert.Empty(getCalendarResult.Dates[1].PreparationTimes);

                Assert.Equal(new DateTime(2000, 01, 03), getCalendarResult.Dates[2].Date);
                Assert.Equal(2, getCalendarResult.Dates[2].Bookings.Count);
                Assert.Contains(
                    getCalendarResult.Dates[2].Bookings,
                    x => x.Id == postBooking1Result.Id && x.Unit == getBooking1Result.Unit);
                Assert.Contains(
                    getCalendarResult.Dates[2].Bookings,
                    x => x.Id == postBooking2Result.Id && x.Unit == getBooking2Result.Unit);
                Assert.Empty(getCalendarResult.Dates[2].PreparationTimes);

                Assert.Equal(new DateTime(2000, 01, 04), getCalendarResult.Dates[3].Date);
                Assert.Single(getCalendarResult.Dates[3].Bookings);
                Assert.Contains(
                    getCalendarResult.Dates[3].Bookings,
                    x => x.Id == postBooking2Result.Id && x.Unit == getBooking2Result.Unit);
                Assert.Single(getCalendarResult.Dates[3].PreparationTimes);
                Assert.Contains(
                    getCalendarResult.Dates[3].PreparationTimes,
                    x => x.Unit == getBooking1Result.Unit);
                
                Assert.Equal(new DateTime(2000, 01, 05), getCalendarResult.Dates[4].Date);
                Assert.Empty(getCalendarResult.Dates[4].Bookings);
                Assert.Single(getCalendarResult.Dates[4].PreparationTimes);
                Assert.Contains(getCalendarResult.Dates[4].PreparationTimes, x => x.Unit == getBooking2Result.Unit);
                
                Assert.Equal(new DateTime(2000, 01, 06), getCalendarResult.Dates[5].Date);
                Assert.Empty(getCalendarResult.Dates[5].Bookings);
                Assert.Empty(getCalendarResult.Dates[5].PreparationTimes);
            }
        }
    }
}
