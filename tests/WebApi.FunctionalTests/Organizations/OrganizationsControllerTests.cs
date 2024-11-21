using System.Net;
using System.Net.Http.Json;
using Contracts.Organizations;
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
        var createOrganizationResponse = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOrganization = await createOrganizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>();
        createdOrganization.Should().NotBeNull();
        createdOrganization?.Id.Should().NotBeEmpty();
        createdOrganization?.Name.Should().Be(createOrganizationRequest.Name);
        createdOrganization?.Description.Should().Be(createOrganizationRequest.Description);
        createOrganizationResponse.Headers.Location.Should().NotBeNull();
        createOrganizationResponse.Headers.Location!.AbsolutePath.Should()
            .Be($"/api/organizations/{createdOrganization?.Id}");
    }

    [Fact]
    public async Task CreateOrganization_WhenNameIsMissing_ShouldReturnBadRequest()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: string.Empty);

        // Act
        var createOrganizationResponse = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);

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
        var createOrganizationResponse = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);

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
        var createOrganizationResponse = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);

        // Assert
        createOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrganization_WhenOrganizationExists_ShouldReturnOrganization()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);
        var createdOrganization = await createOrganizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>();

        // Act
        var getOrganizationResponse = await HttpClient.GetAsync($"api/organizations/{createdOrganization?.Id}");

        // Assert
        getOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedOrganization = await getOrganizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>();
        fetchedOrganization.Should().NotBeNull();
        fetchedOrganization?.Id.Should().NotBeEmpty();
        fetchedOrganization?.Name.Should().Be(createOrganizationRequest.Name);
        fetchedOrganization?.Description.Should().Be(createOrganizationRequest.Description);
    }

    [Fact]
    public async Task GetOrganization_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var organizationId = Guid.NewGuid();

        // Act
        var getOrganizationResponse = await HttpClient.GetAsync($"api/organizations/{organizationId}");

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
        var createOrganizationResponse1 = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest1);
        var createOrganizationResponse2 = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest2);
        var createdOrganization1 = await createOrganizationResponse1.Content.ReadFromJsonAsync<OrganizationResponse>();
        var createdOrganization2 = await createOrganizationResponse2.Content.ReadFromJsonAsync<OrganizationResponse>();

        // Act
        var listOrganizationsResponse = await HttpClient.GetAsync($"api/organizations");

        // Assert
        listOrganizationsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedOrganizations = await listOrganizationsResponse.Content.ReadFromJsonAsync<List<OrganizationResponse>>();
        fetchedOrganizations.Should().NotBeEmpty();
        fetchedOrganizations.Should().ContainSingle(organization => organization.Id == createdOrganization1!.Id);
        fetchedOrganizations.Should().ContainSingle(organization => organization.Id == createdOrganization2!.Id);
    }

    [Fact]
    public async Task UpdateOrganization_WhenOrganizationExists_ShouldUpdatedOrganization()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);
        var createdOrganization = await createOrganizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>();
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest();

        // Act
        var updateOrganizationResponse = await HttpClient.PutAsJsonAsync(
            $"api/organizations/{createdOrganization?.Id}", updateOrganizationRequest);

        // Assert
        updateOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedOrganization = await updateOrganizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>();
        updatedOrganization?.Name.Should().Be(updateOrganizationRequest.Name);
        updatedOrganization?.Description.Should().Be(updateOrganizationRequest.Description);
    }

    [Fact]
    public async Task UpdateOrganization_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var organizationId = Guid.NewGuid();
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest();

        // Act
        var updateOrganizationResponse = await HttpClient.PutAsJsonAsync(
            $"api/organizations/{organizationId}",
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
        var createOrganizationResponse = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);
        var createdOrganization = await createOrganizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>();
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest(name: string.Empty);

        // Act
        var updateOrganizationResponse = await HttpClient.PutAsJsonAsync($"api/organizations/{createdOrganization?.Id}", updateOrganizationRequest);

        // Assert
        updateOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateOrganization_WhenNameIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);
        var createdOrganization = await createOrganizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>();
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest(name: new string('a', 256));

        // Act
        var updateOrganizationResponse = await HttpClient.PutAsJsonAsync($"api/organizations/{createdOrganization?.Id}", updateOrganizationRequest);

        // Assert
        updateOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteOrganization_WhenOrganizationExists_ShouldDeleteOrganization()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await HttpClient.PostAsJsonAsync($"api/organizations", createOrganizationRequest);
        var createdOrganization = await createOrganizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>();

        // Act
        var deleteOrganizationResponse = await HttpClient.DeleteAsync($"api/organizations/{createdOrganization?.Id}");

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
        var deleteOrganizationResponse = await HttpClient.DeleteAsync($"api/organizations/{organizationId}");

        // Assert
        deleteOrganizationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}