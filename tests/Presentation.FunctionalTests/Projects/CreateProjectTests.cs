using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Presentation.FunctionalTests.Common;
using Presentation.FunctionalTests.Organizations;

namespace Presentation.FunctionalTests.Projects;

public class CreateProjectTests : BaseFunctionalTest
{
    public CreateProjectTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateProject_WhenRequestIsValid_ShouldReturnCreated()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await _httpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            createOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);
        var createProjectRequest = ProjectRequestFactory.CreateCreateProjectRequest();

        // Act
        var createProjectResponse = await _httpClient.PostAsJsonAsync(
            Routes.Projects.Create(organizationId),
            createProjectRequest,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        createProjectResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var projectId = await createProjectResponse.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);
        projectId.Should().NotBeEmpty();
        createProjectResponse.Headers.Location.Should().NotBeNull();
        createProjectResponse.Headers.Location!.AbsolutePath.Should()
            .Be(Routes.Projects.Get(projectId));
    }
}