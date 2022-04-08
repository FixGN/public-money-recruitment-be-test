using System;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Contracts.v1.Common;
using VacationRental.Api.Contracts.v1.Rental;
using VacationRental.Api.Tests.Integration.Clients.Models;

namespace VacationRental.Api.Tests.Integration.Clients.v1;

public class RentalsV1Client : IDisposable
{
    private readonly HttpClient _httpClient;

    public RentalsV1Client(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ClientResponseModel<ResourceIdViewModel>> CreateRentalAsync(RentalBindingModel request)
    {
        using var response = await _httpClient.PostAsJsonAsync($"/api/v1/rentals", request);
        ResourceIdViewModel? responseMessage = null;
        if (response.IsSuccessStatusCode)
        {
            responseMessage = await response.Content.ReadAsAsync<ResourceIdViewModel>();
        }
        return new ClientResponseModel<ResourceIdViewModel>(response.IsSuccessStatusCode, response.StatusCode, responseMessage);
    }

    public async Task<ClientResponseModel<RentalViewModel>> GetRentalAsync(int id)
    {
        using var response = await _httpClient.GetAsync($"/api/v1/rentals/{id}");
        RentalViewModel? responseMessage = null;
        if (response.IsSuccessStatusCode)
        {
            responseMessage = await response.Content.ReadAsAsync<RentalViewModel>();
        }
        return new ClientResponseModel<RentalViewModel>(response.IsSuccessStatusCode, response.StatusCode, responseMessage);
    }

    public async Task<ClientResponseModel<RentalViewModel>> UpdateRentalAsync(int id, RentalBindingModel request)
    {
        using var response = await _httpClient.PutAsJsonAsync($"/api/v1/rentals/{id}", request);
        RentalViewModel? responseMessage = null;
        if (response.IsSuccessStatusCode)
        {
            responseMessage = await response.Content.ReadAsAsync<RentalViewModel>();
        }
        return new ClientResponseModel<RentalViewModel>(response.IsSuccessStatusCode, response.StatusCode, responseMessage);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}