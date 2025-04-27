using System.Net;
using System.Net.Http.Json;
using Application.Organizations.Queries.GetOrganization;
using FluentAssertions;
using WebApi.FunctionalTests.Common;

namespace WebApi.FunctionalTests.Organizations;

public class GetOrganizationTests : BaseFunctionalTest
{
    public GetOrganizationTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetOrganization_WhenOrganizationExists_ShouldReturnOrganization()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest, cancellationToken: TestContext.Current.CancellationToken);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>(cancellationToken: TestContext.Current.CancellationToken);

        // Act
        var getOrganizationResponse = await _httpClient.GetAsync($"api/v1/organizations/{organizationId}", TestContext.Current.CancellationToken);

        // Assert
        getOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedOrganization = await getOrganizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>(cancellationToken: TestContext.Current.CancellationToken);
        fetchedOrganization.Should().NotBeNull();
        fetchedOrganization.Id.Should().NotBeEmpty();
        fetchedOrganization.Name.Should().Be(createOrganizationRequest.Name);
        fetchedOrganization.Description.Should().Be(createOrganizationRequest.Description);
    }

    [Fact]
    public async Task GetOrganization_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var organizationId = Guid.NewGuid();

        // Act
        var getOrganizationResponse = await _httpClient.GetAsync($"api/v1/organizations/{organizationId}", TestContext.Current.CancellationToken);

        // Assert
        getOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}