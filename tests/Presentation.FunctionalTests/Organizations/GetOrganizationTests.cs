using System.Net.Http.Json;
using Application.Organizations.Queries.GetOrganization;
using FluentAssertions;
using Presentation.FunctionalTests.Infrastructure;

namespace Presentation.FunctionalTests.Organizations;

public class GetOrganizationTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task GetOrganization_WhenOrganizationExists_ShouldReturnOrganization()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            createOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);

        // Act
        var getOrganizationResponse = await fixture.HttpClient.GetAsync(
            Routes.Organizations.Get(organizationId),
            TestContext.Current.CancellationToken);

        // Assert
        getOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedOrganization = await getOrganizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        fetchedOrganization.Should().NotBeNull();
        fetchedOrganization.Id.Should().NotBeEmpty();
        fetchedOrganization.Name.Should().Be(createOrganizationRequest.Name);
        fetchedOrganization.Description.Should().Be(createOrganizationRequest.Description);
    }

    [Fact]
    public async Task GetOrganization_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var organizationId = Guid.NewGuid();

        // Act
        var getOrganizationResponse = await fixture.HttpClient.GetAsync(
            Routes.Organizations.Get(organizationId),
            TestContext.Current.CancellationToken);

        // Assert
        getOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}