using System.Net.Http.Json;
using FluentAssertions;
using Presentation.Controllers.Projects;
using Presentation.FunctionalTests.Infrastructure;
using Presentation.FunctionalTests.Organizations;

namespace Presentation.FunctionalTests.Projects;

public class CreateProjectTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task CreateProject_WhenRequestIsValid_ShouldReturnCreated()
    {
        // Arrange
        var createOrganizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var createOrganizationResponse = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            createOrganizationRequest,
            cancellationToken: TestContext.Current.CancellationToken);
        var organizationId = await createOrganizationResponse.Content.ReadFromJsonAsync<Guid>(
            cancellationToken: TestContext.Current.CancellationToken);
        var createProjectRequest = new CreateProjectRequest(
            "Project Name",
            "Project Description");

        // Act
        var createProjectResponse = await fixture.HttpClient.PostAsJsonAsync(
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