using System.Net;
using System.Net.Http.Json;
using Application.Organizations.Queries.GetOrganization;
using FluentAssertions;
using WebApi.FunctionalTests.Common;

namespace WebApi.FunctionalTests.Organizations;

public class ListOrganizationsTests : BaseFunctionalTest
{
    public ListOrganizationsTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ListOrganizations_WhenOrganizationsExist_ShouldReturnOrganizations()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest1 = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: "Organization 1");
        var createOrganizationRequest2 = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: "Organization 2");
        var createOrganizationResponse1 = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest1);
        var createOrganizationResponse2 = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest2);
        var organizationId1 = await createOrganizationResponse1.Content.ReadFromJsonAsync<Guid>();
        var organizationId2 = await createOrganizationResponse2.Content.ReadFromJsonAsync<Guid>();

        // Act
        var listOrganizationsResponse = await _httpClient.GetAsync("api/v1/organizations");

        // Assert
        listOrganizationsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedOrganizations = await listOrganizationsResponse.Content.ReadFromJsonAsync<List<OrganizationResponse>>();
        fetchedOrganizations.Should().NotBeEmpty();
        fetchedOrganizations.Should().ContainSingle(organization => organization.Id == organizationId1);
        fetchedOrganizations.Should().ContainSingle(organization => organization.Id == organizationId2);
    }

    [Fact]
    public async Task DeleteOrganization_WhenOrganizationExists_ShouldDeleteOrganization()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        var deleteOrganizationResponse = await _httpClient.DeleteAsync($"api/v1/organizations/{organizationId}");

        // Assert
        deleteOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrganization_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var organizationId = Guid.NewGuid();

        // Act
        var deleteOrganizationResponse = await _httpClient.DeleteAsync($"api/v1/organizations/{organizationId}");

        // Assert
        deleteOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}