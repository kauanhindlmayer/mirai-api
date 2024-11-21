using System.Net;
using System.Net.Http.Json;
using Contracts.Organizations;
using Contracts.Projects;
using FluentAssertions;
using WebApi.FunctionalTests.Common;
using WebApi.FunctionalTests.Organizations;

namespace WebApi.FunctionalTests.Projects;

public class ProjectsControllerTests : BaseFunctionalTest
{
    public ProjectsControllerTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateProject_WhenRequestIsValid_ShouldReturnCreatedProject()
    {
        // Arrange
        await SetAuthorizationHeaderAsync();
        var organizationRequest = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var organizationResponse = await HttpClient.PostAsJsonAsync("/api/organizations", organizationRequest);
        var createdOrganization = await organizationResponse.Content.ReadFromJsonAsync<OrganizationResponse>();
        var createProjectRequest = ProjectRequestFactory.CreateCreateProjectRequest();

        // Act
        var createProjectResponse = await HttpClient.PostAsJsonAsync(
            $"/api/organizations/{createdOrganization?.Id}/projects",
            createProjectRequest);

        // Assert
        createProjectResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProject = await createProjectResponse.Content.ReadFromJsonAsync<ProjectResponse>();
        createdProject.Should().NotBeNull();
        createdProject?.Id.Should().NotBeEmpty();
        createdProject?.Name.Should().Be(createProjectRequest.Name);
        createdProject?.Description.Should().Be(createProjectRequest.Description);
        createdProject?.OrganizationId.Should().Be(createdOrganization!.Id);
        createProjectResponse.Headers.Location.Should().NotBeNull();
        createProjectResponse.Headers.Location!.AbsolutePath.Should()
            .Be($"/api/organizations/{createdOrganization?.Id}/projects/{createdProject?.Id}");
    }
}