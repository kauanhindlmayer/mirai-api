using Mirai.Api.IntegrationTests.Common.Organizations;
using Mirai.Api.IntegrationTests.Common.WebApplicationFactory;

namespace Mirai.Api.IntegrationTests.Controllers;

[Collection(WebAppFactoryCollection.CollectionName)]
public class OrganizationsControllerTests
{
    private readonly AppHttpClient _client;

    public OrganizationsControllerTests(WebAppFactory webAppFactory)
    {
        _client = webAppFactory.CreateAppHttpClient();
        webAppFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateOrganization_WhenRequestIsValid_ShouldReturnCreatedOrganization()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var token = await _client.GenerateTokenAsync();

        // Act
        var response = await _client.CreateOrganizationAndExpectSuccessAsync(createOrganizationRequest, token);

        // Assert
        response.Id.Should().NotBeEmpty();
        response.Name.Should().Be(createOrganizationRequest.Name);
        response.Description.Should().Be(createOrganizationRequest.Description);
    }

    [Fact]
    public async Task CreateOrganization_WhenNameIsMissing_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: string.Empty);

        // Act
        var response = await _client.CreateOrganizationAsync(createOrganizationRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrganization_WhenNameIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: new string('a', 256));

        // Act
        var response = await _client.CreateOrganizationAsync(createOrganizationRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrganization_WhenDescriptionIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest(description: new string('a', 501));

        // Act
        var response = await _client.CreateOrganizationAsync(createOrganizationRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrganization_WhenOrganizationExists_ShouldReturnOrganization()
    {
        // Arrange
        var token = await _client.GenerateTokenAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createdOrganization = await _client.CreateOrganizationAndExpectSuccessAsync(createOrganizationRequest, token);

        // Act
        var response = await _client.GetOrganizationAndExpectSuccessAsync(createdOrganization.Id, token);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(createdOrganization.Id);
        response.Name.Should().Be(createdOrganization.Name);
        response.Description.Should().Be(createdOrganization.Description);
    }

    [Fact]
    public async Task GetOrganization_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.GetOrganizationAsync(id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ListOrganizations_WhenOrganizationsExist_ShouldReturnOrganizations()
    {
        // Arrange
        var token = await _client.GenerateTokenAsync();
        var createOrganizationRequest1 = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: "Organization 1");
        var createOrganizationRequest2 = OrganizationRequestFactory.CreateCreateOrganizationRequest(name: "Organization 2");
        var createdOrganization1 = await _client.CreateOrganizationAndExpectSuccessAsync(createOrganizationRequest1, token);
        var createdOrganization2 = await _client.CreateOrganizationAndExpectSuccessAsync(createOrganizationRequest2, token);

        // Act
        var response = await _client.ListOrganizationsAndExpectSuccessAsync(token);

        // Assert
        response.Should().NotBeEmpty();
        response.Should().ContainSingle(organization => organization.Id == createdOrganization1.Id);
        response.Should().ContainSingle(organization => organization.Id == createdOrganization2.Id);
    }

    [Fact]
    public async Task UpdateOrganization_WhenOrganizationExists_ShouldUpdatedOrganization()
    {
        // Arrange
        var token = await _client.GenerateTokenAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createdOrganization = await _client.CreateOrganizationAndExpectSuccessAsync(createOrganizationRequest, token);
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest();

        // Act
        var response = await _client.UpdateOrganizationAndExpectSuccessAsync(createdOrganization.Id, updateOrganizationRequest, token);

        // Assert
        response.Id.Should().Be(createdOrganization.Id);
        response.Name.Should().Be(updateOrganizationRequest.Name);
        response.Description.Should().Be(updateOrganizationRequest.Description);
    }

    [Fact]
    public async Task UpdateOrganization_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest();

        // Act
        var response = await _client.UpdateOrganizationAsync(id, updateOrganizationRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOrganization_WhenNameIsMissing_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await _client.GenerateTokenAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createdOrganization = await _client.CreateOrganizationAndExpectSuccessAsync(createOrganizationRequest, token);
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest(name: string.Empty);

        // Act
        var response = await _client.UpdateOrganizationAsync(createdOrganization.Id, updateOrganizationRequest, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateOrganization_WhenNameIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await _client.GenerateTokenAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createdOrganization = await _client.CreateOrganizationAndExpectSuccessAsync(createOrganizationRequest, token);
        var updateOrganizationRequest = OrganizationRequestFactory.CreateUpdateOrganizationRequest(name: new string('a', 256));

        // Act
        var response = await _client.UpdateOrganizationAsync(createdOrganization.Id, updateOrganizationRequest, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteOrganization_WhenOrganizationExists_ShouldDeleteOrganization()
    {
        // Arrange
        var token = await _client.GenerateTokenAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createdOrganization = await _client.CreateOrganizationAndExpectSuccessAsync(createOrganizationRequest, token);

        // Act
        var response = await _client.DeleteOrganizationAsync(createdOrganization.Id, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrganization_WhenOrganizationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.DeleteOrganizationAsync(id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}