using System.Net;
using System.Net.Http.Json;
using Application.Organizations.Queries.GetOrganization;
using FluentAssertions;
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
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();

        // Act
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync(
            "api/v1/organizations",
            createOrganizationRequest);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>();
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
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync(
            "api/v1/organizations",
            createOrganizationRequest);

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
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync(
            "api/v1/organizations",
            createOrganizationRequest);

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
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync(
            "api/v1/organizations",
            createOrganizationRequest);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrganization_WhenOrganizationExists_ShouldReturnOrganization()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        var getOrganizationResponse = await _httpClient.GetAsync($"api/v1/organizations/{organizationId}");

        // Assert
        getOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedOrganization = await getOrganizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>();
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
        var getOrganizationResponse = await _httpClient.GetAsync($"api/v1/organizations/{organizationId}");

        // Assert
        getOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
    public async Task UpdateOrganization_WhenOrganizationExists_ShouldReturnOk()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>();
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest();

        // Act
        var updateOrganizationResponse = await _httpClient.PutAsJsonAsync(
            $"api/v1/organizations/{organizationId}", updateOrganizationRequest);

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
            updateOrganizationRequest);

        // Assert
        updateOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOrganization_WhenNameIsMissing_ShouldReturnBadRequest()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>();
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest(name: string.Empty);

        // Act
        var updateOrganizationResponse = await _httpClient.PutAsJsonAsync($"api/v1/organizations/{organizationId}", updateOrganizationRequest);

        // Assert
        updateOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateOrganization_WhenNameIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync("api/v1/organizations", createOrganizationRequest);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>();
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest(name: new string('a', 256));

        // Act
        var updateOrganizationResponse = await _httpClient.PutAsJsonAsync($"api/v1/organizations/{organizationId}", updateOrganizationRequest);

        // Assert
        updateOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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