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
        var createOrganizationRequest = OrganizationRequestFactory.CreateOrganizationRequest();

        // Act
        var response = await _client.CreateOrganizationAndExpectSuccessAsync(createOrganizationRequest);

        // Assert
        response.Id.Should().NotBeEmpty();
        response.Name.Should().Be(createOrganizationRequest.Name);
        response.Description.Should().Be(createOrganizationRequest.Description);
    }

    [Fact]
    public async Task CreateOrganization_WhenNameIsMissing_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateOrganizationRequest(name: string.Empty);

        // Act
        var response = await _client.CreateOrganizationAsync(createOrganizationRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrganization_WhenNameIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateOrganizationRequest(name: new string('a', 256));

        // Act
        var response = await _client.CreateOrganizationAsync(createOrganizationRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrganization_WhenDescriptionIsTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateOrganizationRequest(description: new string('a', 501));

        // Act
        var response = await _client.CreateOrganizationAsync(createOrganizationRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrganization_WhenOrganizationExists_ShouldReturnOrganization()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateOrganizationRequest();
        var createdOrganization = await _client.CreateOrganizationAndExpectSuccessAsync(createOrganizationRequest);

        // Act
        var response = await _client.GetOrganizationAndExpectSuccessAsync(createdOrganization.Id);

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