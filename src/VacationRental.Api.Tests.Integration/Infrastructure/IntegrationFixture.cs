using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using VacationRental.Api.Tests.Integration.Clients.v1;
using Xunit;

namespace VacationRental.Api.Tests.Integration.Infrastructure;

[CollectionDefinition("Integration")]
public sealed class IntegrationFixture : IDisposable, ICollectionFixture<IntegrationFixture>
{
    private readonly TestServer _server;

    public IntegrationFixture()
    {
        _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());

        BookingsV1Client = new BookingsV1Client(_server.CreateClient());
        RentalsV1Client = new RentalsV1Client(_server.CreateClient());
        CalendarV1Client = new CalendarV1Client(_server.CreateClient());
    }

    public BookingsV1Client BookingsV1Client { get; }
    public RentalsV1Client RentalsV1Client { get; }
    public CalendarV1Client CalendarV1Client { get; }

    public void Dispose()
    {
        _server.Dispose();
        BookingsV1Client.Dispose();
        RentalsV1Client.Dispose();
        CalendarV1Client.Dispose();
    }
}