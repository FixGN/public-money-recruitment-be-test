using System.Threading.Tasks;
using VacationRental.Api.Contracts.Rental;
using VacationRental.Api.Tests.Integration.Clients;
using VacationRental.Api.Tests.Integration.Infrastructure;
using Xunit;

namespace VacationRental.Api.Tests.Integration.Endpoints.Rentals
{
    [Collection("Integration")]
    public class PostRentalTests
    {
        private readonly RentalsClient _rentalsClient;

        public PostRentalTests(IntegrationFixture fixture)
        {
            _rentalsClient = fixture.RentalsClient;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostRental_ThenAGetReturnsTheCreatedRental()
        {
            var createRequest = new RentalBindingModel(25, 1);
            var createResponse = await _rentalsClient.CreateRentalAsync(createRequest);
            Assert.True(createResponse.IsSuccessStatusCode);
            
            var getResponse = await _rentalsClient.GetRentalAsync(createResponse.Message!.Id);
            Assert.True(getResponse.IsSuccessStatusCode);
            
            Assert.Equal(createRequest.Units, getResponse.Message!.Units);
            Assert.Equal(createRequest.PreparationTimeInDays, getResponse.Message!.PreparationTimeInDays);
        }
    }
}
