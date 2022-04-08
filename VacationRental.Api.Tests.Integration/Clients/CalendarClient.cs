using System;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Contracts.Calendar;
using VacationRental.Api.Tests.Integration.Clients.Models;

namespace VacationRental.Api.Tests.Integration.Clients;

public class CalendarClient
{
    private readonly HttpClient _httpClient;

    public CalendarClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<ClientResponseModel<CalendarViewModel>> GetCalendarAsync(int rentalId, DateTime start, int nights)
    {
        using var response = await _httpClient
            .GetAsync($"/api/v1/calendar?rentalId={rentalId}&start={start}&nights={nights}");

        CalendarViewModel? responseMessage = null;
        if (response.IsSuccessStatusCode)
        {
            responseMessage = await response.Content.ReadAsAsync<CalendarViewModel>();
        }
        return new ClientResponseModel<CalendarViewModel>(response.IsSuccessStatusCode, response.StatusCode, responseMessage);
    }
}