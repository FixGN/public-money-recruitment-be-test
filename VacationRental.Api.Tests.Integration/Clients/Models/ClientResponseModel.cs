using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace VacationRental.Api.Tests.Integration.Clients.Models;

public class ClientResponseModel<T> where T : class
{
    public ClientResponseModel(bool isSuccessStatusCode, HttpStatusCode statusCode, T? message)
    {
        IsSuccessStatusCode = isSuccessStatusCode;
        StatusCode = statusCode;
        Message = message;
    }

    [MemberNotNullWhen(true, nameof(Message))]
    public bool IsSuccessStatusCode { get; }
    public HttpStatusCode StatusCode { get; }
    public T? Message { get; }
}