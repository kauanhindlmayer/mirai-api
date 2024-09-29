using Mirai.Api.IntegrationTests.Common.Organizations;
using Mirai.Api.IntegrationTests.Common.Projects;
using Mirai.Api.IntegrationTests.Common.WebApplicationFactory;

namespace Mirai.Api.IntegrationTests.Controllers;

[Collection(WebAppFactoryCollection.CollectionName)]
public class ProjectsControllerTests
{
    private readonly AppHttpClient _client;

    public ProjectsControllerTests(WebAppFactory webAppFactory)
    {
        _client = webAppFactory.CreateAppHttpClient();
        webAppFactory.ResetDatabase();
    }

    [Fact]
    public async Task CreateProject_WhenRequestIsValid_ShouldReturnCreatedProject()
    {
        // Arrange
        var token = await _client.GenerateTokenAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var organization = await _client.CreateOrganizationAndExpectSuccessAsync(createOrganizationRequest, token);
        var createProjectRequest = ProjectRequestFactory.CreateCreateProjectRequest();

        // Act
        var response = await _client.CreateProjectAndExpectSuccessAsync(createProjectRequest, organization.Id, token);

        // Assert
        response.Id.Should().NotBeEmpty();
        response.Name.Should().Be(createProjectRequest.Name);
        response.Description.Should().Be(createProjectRequest.Description);
    }
}