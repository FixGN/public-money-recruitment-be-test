using System.Threading.Tasks;
using VacationRental.Api.Contracts.Rental;
using VacationRental.Api.Tests.Integration.Clients;
using VacationRental.Api.Tests.Integration.Clients.v1;
using VacationRental.Api.Tests.Integration.Infrastructure;
using Xunit;

namespace VacationRental.Api.Tests.Integration.Endpoints.Rentals
{
    [Collection("Integration")]
    public class PostRentalTests
    {
        private readonly RentalsV1Client _rentalsV1Client;

        public PostRentalTests(IntegrationFixture fixture)
        {
            _rentalsV1Client = fixture.RentalsV1Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostRental_ThenAGetReturnsTheCreatedRental()
        {
            var createRequest = new RentalBindingModel(25, 1);
            var createResponse = await _rentalsV1Client.CreateRentalAsync(createRequest);
            Assert.True(createResponse.IsSuccessStatusCode);
            
            var getResponse = await _rentalsV1Client.GetRentalAsync(createResponse.Message!.Id);
            Assert.True(getResponse.IsSuccessStatusCode);
            
            Assert.Equal(createRequest.Units, getResponse.Message!.Units);
            Assert.Equal(createRequest.PreparationTimeInDays, getResponse.Message!.PreparationTimeInDays);
        }
    }
}
