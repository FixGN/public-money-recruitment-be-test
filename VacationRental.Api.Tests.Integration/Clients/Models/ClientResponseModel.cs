using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace VacationRental.Api.Tests.Integration.Clients.Models;

public class ClientResponseModel<T> where T : class
{
    public ClientResponseModel(bool isSuccess, HttpStatusCode statusCode, T? message)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Message = message;
    }

    [MemberNotNullWhen(true, nameof(Message))]
    public bool IsSuccess { get; }
    public HttpStatusCode StatusCode { get; }
    public T? Message { get; }
}