using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using WebApi.FunctionalTests.Common;

namespace WebApi.FunctionalTests.Organizations;

public class UpdateOrganizationTests : BaseFunctionalTest
{
    public UpdateOrganizationTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task UpdateOrganization_WhenOrganizationExists_ShouldReturnOk()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync(
            "api/v1/organizations",
            createOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest();

        // Act
        var updateOrganizationResponse = await _httpClient.PutAsJsonAsync(
            $"api/v1/organizations/{organizationId}",
            updateOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        updateOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateOrganization_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var organizationId = Guid.NewGuid();
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest();

        // Act
        var updateOrganizationResponse = await _httpClient.PutAsJsonAsync(
            $"api/v1/organizations/{organizationId}",
            updateOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        updateOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOrganization_WhenNameIsMissing_ShouldReturnBadRequest()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: "Organization 2");
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync(
            "api/v1/organizations",
            createOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest(name: string.Empty);

        // Act
        var updateOrganizationResponse = await _httpClient.PutAsJsonAsync(
            $"api/v1/organizations/{organizationId}",
            updateOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        updateOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateOrganization_WhenNameIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync(
            "api/v1/organizations",
            createOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest(name: new string('a', 256));

        // Act
        var updateOrganizationResponse = await _httpClient.PutAsJsonAsync(
            $"api/v1/organizations/{organizationId}",
            updateOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        updateOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}