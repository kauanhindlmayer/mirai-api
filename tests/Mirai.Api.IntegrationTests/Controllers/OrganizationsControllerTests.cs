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
}