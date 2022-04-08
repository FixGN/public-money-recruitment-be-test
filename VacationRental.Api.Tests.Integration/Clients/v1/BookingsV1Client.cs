using System;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Contracts.Booking;
using VacationRental.Api.Contracts.Common;
using VacationRental.Api.Tests.Integration.Clients.Models;

namespace VacationRental.Api.Tests.Integration.Clients.v1;

public class BookingsV1Client : IDisposable
{
    private readonly HttpClient _httpClient;

    public BookingsV1Client(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ClientResponseModel<ResourceIdViewModel>> CreateBookingAsync(BookingBindingModel request)
    {
        using var response = await _httpClient.PostAsJsonAsync($"/api/v1/bookings", request);
        ResourceIdViewModel? responseMessage = null;
        if (response.IsSuccessStatusCode)
        {
            responseMessage = await response.Content.ReadAsAsync<ResourceIdViewModel>();
        }
        return new ClientResponseModel<ResourceIdViewModel>(response.IsSuccessStatusCode, response.StatusCode, responseMessage);
    }

    public async Task<ClientResponseModel<BookingViewModel>> GetBookingAsync(int id)
    {
        using var response = await _httpClient.GetAsync($"/api/v1/bookings/{id}");
        BookingViewModel? responseMessage = null;
        if (response.IsSuccessStatusCode)
        {
            responseMessage = await response.Content.ReadAsAsync<BookingViewModel>();
        }
        return new ClientResponseModel<BookingViewModel>(response.IsSuccessStatusCode, response.StatusCode, responseMessage);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}