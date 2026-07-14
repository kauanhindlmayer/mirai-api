using System.Net;
using System.Net.Http.Json;
using Application.Users.Queries.GetCurrentUser;
using Application.Users.Queries.GetUserProfile;
using FluentAssertions;
using Presentation.Controllers.Organizations;
using Presentation.Controllers.Projects;
using Presentation.Controllers.Teams;
using Presentation.FunctionalTests.Infrastructure;
using Presentation.FunctionalTests.Users;

namespace Presentation.FunctionalTests.Organizations;

public class GetUserProfileTests(DistributedApplicationTestFixture fixture)
{
    [Fact]
    public async Task GetUserProfile_WhenUserIsOrganizationMember_ShouldReturnProfileScopedToOrganization()
    {
        // Arrange
        var userId = await GetSecondaryUserIdAsync();
        var organizationId = await CreateOrganizationAsync();
        await AddSecondaryUserToOrganizationAsync(organizationId);
        var projectId = await CreateProjectAsync(organizationId, "Apollo");
        await AddUserToProjectAsync(projectId, userId);
        var teamId = await CreateTeamAsync(projectId, "Backend");
        await AddUserToTeamAsync(projectId, teamId, userId);

        // Act
        var response = await fixture.HttpClient.GetAsync(
            Routes.Organizations.GetUserProfile(organizationId, userId),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        profile.Should().NotBeNull();
        profile.Id.Should().Be(userId);
        profile.Email.Should().Be(UserRequestFactory.SecondaryEmail);
        profile.FullName.Should().Be("Jane Smith");
        profile.AvatarUrl.Should().BeNull();
        profile.Projects.Should().ContainSingle();
        profile.Projects[0].Id.Should().Be(projectId);
        profile.Projects[0].Name.Should().Be("Apollo");
        profile.Projects[0].RoleName.Should().Be("Contributor");
        profile.Teams.Should().ContainSingle();
        profile.Teams[0].Id.Should().Be(teamId);
        profile.Teams[0].Name.Should().Be("Backend");
        profile.Teams[0].ProjectName.Should().Be("Apollo");
        profile.Teams[0].RoleName.Should().Be("Member");
    }

    [Fact]
    public async Task GetUserProfile_WhenUserHasNoProjectsOrTeamsInOrganization_ShouldReturnEmptyLists()
    {
        // Arrange
        var userId = await GetSecondaryUserIdAsync();
        var organizationId = await CreateOrganizationAsync();
        await AddSecondaryUserToOrganizationAsync(organizationId);

        // Act
        var response = await fixture.HttpClient.GetAsync(
            Routes.Organizations.GetUserProfile(organizationId, userId),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        profile.Should().NotBeNull();
        profile.Projects.Should().BeEmpty();
        profile.Teams.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserProfile_WhenTargetUserIsNotOrganizationMember_ShouldReturnNotFound()
    {
        // Arrange
        var userId = await GetSecondaryUserIdAsync();
        var organizationId = await CreateOrganizationAsync();

        // Act
        var response = await fixture.HttpClient.GetAsync(
            Routes.Organizations.GetUserProfile(organizationId, userId),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserProfile_WhenCallerIsNotOrganizationMember_ShouldReturnForbidden()
    {
        // Arrange
        var userId = await GetSecondaryUserIdAsync();
        var organizationId = await CreateOrganizationAsync();

        // Act
        var response = await fixture.SecondaryHttpClient.GetAsync(
            Routes.Organizations.GetUserProfile(organizationId, userId),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserProfile_WhenUnauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var organizationId = await CreateOrganizationAsync();

        // Act
        var response = await fixture.UnauthenticatedHttpClient.GetAsync(
            Routes.Organizations.GetUserProfile(organizationId, Guid.NewGuid()),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<Guid> GetSecondaryUserIdAsync()
    {
        var response = await fixture.SecondaryHttpClient.GetAsync(
            Routes.Users.GetLoggedInUser,
            TestContext.Current.CancellationToken);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        return user!.Id;
    }

    private async Task<Guid> CreateOrganizationAsync()
    {
        var request = OrganizationRequestFactory.CreateCreateOrganizationRequest();
        var response = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.Create,
            request,
            TestContext.Current.CancellationToken);
        return await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
    }

    private async Task AddSecondaryUserToOrganizationAsync(Guid organizationId)
    {
        var request = new AddUserToOrganizationRequest(UserRequestFactory.SecondaryEmail);
        var response = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Organizations.AddUser(organizationId),
            request,
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<Guid> CreateProjectAsync(Guid organizationId, string name)
    {
        var request = new CreateProjectRequest(name, "Project Description");
        var response = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Projects.Create(organizationId),
            request,
            TestContext.Current.CancellationToken);
        return await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
    }

    private async Task AddUserToProjectAsync(Guid projectId, Guid userId)
    {
        var request = new AddUserToProjectRequest(userId);
        var response = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Projects.AddUser(projectId),
            request,
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<Guid> CreateTeamAsync(Guid projectId, string name)
    {
        var request = new CreateTeamRequest(name, "Team Description");
        var response = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Teams.Create(projectId),
            request,
            TestContext.Current.CancellationToken);
        return await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
    }

    private async Task AddUserToTeamAsync(Guid projectId, Guid teamId, Guid userId)
    {
        var request = new AddUserToTeamRequest(userId);
        var response = await fixture.HttpClient.PostAsJsonAsync(
            Routes.Teams.AddMember(projectId, teamId),
            request,
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
