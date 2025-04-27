using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using WebApi.FunctionalTests.Common;

namespace WebApi.FunctionalTests.Organizations;

public class CreateOrganizationTests : BaseFunctionalTest
{
    public CreateOrganizationTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateOrganization_WhenRequestIsValid_ShouldReturnCreatedOrganization()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();

        // Act
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>(cancellationToken: TestContext.Current.CancellationToken);
        organizationId.Should().NotBeEmpty();
        createOrganizationResponse.Headers.Location.Should().NotBeNull();
        createOrganizationResponse.Headers.Location.AbsolutePath.Should()
            .Be($"/api/v1/organizations/{organizationId}");
    }

    [Fact]
    public async Task CreateOrganization_WhenNameIsMissing_ShouldReturnBadRequest()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: string.Empty);

        // Act
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrganization_WhenNameIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: new string('a', 256));

        // Act
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrganization_WhenDescriptionIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(description: new string('a', 501));

        // Act
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}