using Application.Abstractions.Authorization;
using Application.IntegrationTests.Infrastructure;
using Application.Projects.Queries.ResolveProjectUsers;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Projects.Queries;

public class ResolveProjectUsersTests : BaseIntegrationTest
{
    public ResolveProjectUsersTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ResolveProjectUsers_WhenUserIsNoLongerAProjectMember_ShouldStillResolve()
    {
        // Arrange
        var (organization, project, admin) = await CreateProjectAsync();

        var formerMember = new User("Jane", "Smith", $"jane-{Guid.NewGuid()}@mirai.com");
        formerMember.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(formerMember);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(formerMember, await GetRoleAsync(SystemRoles.OrganizationMemberId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);

        // Act
        var result = await _sender.Send(
            new ResolveProjectUsersQuery(project.Id, [formerMember.Id]),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().ContainSingle(u => u.Id == formerMember.Id && u.FullName == "Jane Smith");
    }

    [Fact]
    public async Task ResolveProjectUsers_WhenUserIsNotInTheOrganization_ShouldOmitFromResults()
    {
        // Arrange
        var (_, project, admin) = await CreateProjectAsync();

        var outsider = new User("Outside", "User", $"outsider-{Guid.NewGuid()}@mirai.com");
        outsider.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(outsider);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);

        // Act
        var result = await _sender.Send(
            new ResolveProjectUsersQuery(project.Id, [outsider.Id]),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task ResolveProjectUsers_WhenUserIdDoesNotExist_ShouldOmitFromResults()
    {
        // Arrange
        var (_, project, admin) = await CreateProjectAsync();
        SetCurrentUser(admin.Id);

        // Act
        var result = await _sender.Send(
            new ResolveProjectUsersQuery(project.Id, [Guid.NewGuid()]),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task ResolveProjectUsers_WhenCallerLacksProjectAccess_ShouldReturnForbidden()
    {
        // Arrange
        var (_, project, _) = await CreateProjectAsync();

        var outsider = new User("Outside", "User", $"outsider-{Guid.NewGuid()}@mirai.com");
        outsider.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(outsider);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(outsider.Id);

        // Act
        var result = await _sender.Send(
            new ResolveProjectUsersQuery(project.Id, [Guid.NewGuid()]),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AuthorizationErrors.Forbidden);
    }

    private async Task<(Organization Organization, Project Project, User Admin)> CreateProjectAsync()
    {
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var admin = new User("Admin", "User", $"admin-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(admin, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        project.AddMember(admin, await GetRoleAsync(SystemRoles.ProjectAdminId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return (organization, project, admin);
    }
}
