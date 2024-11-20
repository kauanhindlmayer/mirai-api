using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Contracts.Organizations;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.FunctionalTests.Common;

namespace WebApi.FunctionalTests.Organizations;

public class OrganizationsControllerTests : BaseFunctionalTest
{
    public OrganizationsControllerTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateOrganization_WhenRequestIsValid_ShouldReturnCreatedOrganization()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);

        // Act
        var response = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var organizationResponse = await response.Content.ReadFromJsonAsync<OrganizationResponse>();
        organizationResponse.Should().NotBeNull();
        organizationResponse?.Id.Should().NotBeEmpty();
        organizationResponse?.Name.Should().Be(createOrganizationRequest.Name);
        organizationResponse?.Description.Should().Be(createOrganizationRequest.Description);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.AbsolutePath.Should().Be($"/api/organizations/{organizationResponse?.Id}");
    }

    [Fact]
    public async Task CreateOrganization_WhenNameIsMissing_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: string.Empty);
        var accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);

        // Act
        var response = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}