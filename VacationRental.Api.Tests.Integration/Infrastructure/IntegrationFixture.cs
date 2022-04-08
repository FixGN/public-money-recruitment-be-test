using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using VacationRental.Api.Tests.Integration.Clients;
using Xunit;

namespace VacationRental.Api.Tests.Integration.Infrastructure
{
    [CollectionDefinition("Integration")]
    public sealed class IntegrationFixture : IDisposable, ICollectionFixture<IntegrationFixture>
    {
        private readonly TestServer _server;

        public HttpClient Client { get; }
        public BookingsClient BookingsClient { get; }
        public RentalsClient RentalsClient { get; }
        public CalendarClient CalendarClient { get; }

        public IntegrationFixture()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());

            Client = _server.CreateClient();
            BookingsClient = new BookingsClient(_server.CreateClient());
            RentalsClient = new RentalsClient(_server.CreateClient());
            CalendarClient = new CalendarClient(_server.CreateClient());
        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}
