using Application.Abstractions.Authorization;
using Application.IntegrationTests.Infrastructure;
using Application.Projects.Queries.GetMentionableProjectUsers;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Projects.Queries;

public class GetMentionableProjectUsersTests : BaseIntegrationTest
{
    public GetMentionableProjectUsersTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetMentionableProjectUsers_WhenUserIsAnOrganizationOwnerWithoutDirectMembership_ShouldIncludeUser()
    {
        // Arrange
        var (organization, project, _) = await CreateProjectAsync();

        var owner = new User("Olivia", "Owner", $"owner-{Guid.NewGuid()}@mirai.com");
        owner.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(owner);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(owner, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await _sender.Send(
            new GetMentionableProjectUsersQuery(project.Id),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().Contain(u => u.Id == owner.Id && u.FullName == "Olivia Owner");
    }

    [Fact]
    public async Task GetMentionableProjectUsers_WhenUserIsAnOrganizationMemberWithoutProjectAccess_ShouldOmitUser()
    {
        // Arrange
        var (organization, project, _) = await CreateProjectAsync();

        var member = new User("Mallory", "Member", $"member-{Guid.NewGuid()}@mirai.com");
        member.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(member);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(member, await GetRoleAsync(SystemRoles.OrganizationMemberId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await _sender.Send(
            new GetMentionableProjectUsersQuery(project.Id),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().NotContain(u => u.Id == member.Id);
    }

    [Fact]
    public async Task GetMentionableProjectUsers_WhenUserIsADirectProjectMember_ShouldIncludeUser()
    {
        // Arrange
        var (organization, project, _) = await CreateProjectAsync();

        var contributor = new User("Casey", "Contributor", $"contributor-{Guid.NewGuid()}@mirai.com");
        contributor.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(contributor);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(contributor, await GetRoleAsync(SystemRoles.OrganizationMemberId));
        project.AddMember(contributor, await GetRoleAsync(SystemRoles.ProjectContributorId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await _sender.Send(
            new GetMentionableProjectUsersQuery(project.Id),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().Contain(u => u.Id == contributor.Id && u.FullName == "Casey Contributor");
    }

    [Fact]
    public async Task GetMentionableProjectUsers_WhenUserHasBothImplicitAndDirectAccess_ShouldIncludeUserOnce()
    {
        // Arrange
        var (organization, project, _) = await CreateProjectAsync();

        var admin = new User("Andy", "Admin", $"andy-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(admin, await GetRoleAsync(SystemRoles.OrganizationAdminId));
        project.AddMember(admin, await GetRoleAsync(SystemRoles.ProjectViewerId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await _sender.Send(
            new GetMentionableProjectUsersQuery(project.Id),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().ContainSingle(u => u.Id == admin.Id);
    }

    [Fact]
    public async Task GetMentionableProjectUsers_WhenSearchTermMatchesName_ShouldFilterResults()
    {
        // Arrange
        var (organization, project, _) = await CreateProjectAsync();

        var target = new User("Zelda", "Zephyr", $"zelda-{Guid.NewGuid()}@mirai.com");
        target.SetIdentityId(Guid.NewGuid().ToString());
        var other = new User("Wendy", "Winters", $"wendy-{Guid.NewGuid()}@mirai.com");
        other.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.AddRange(target, other);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(target, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        organization.AddMember(other, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await _sender.Send(
            new GetMentionableProjectUsersQuery(project.Id, SearchTerm: "zeld"),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().ContainSingle(u => u.Id == target.Id);
    }

    [Fact]
    public async Task GetMentionableProjectUsers_WhenCallerLacksProjectAccess_ShouldReturnForbidden()
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
            new GetMentionableProjectUsersQuery(project.Id),
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

        SetCurrentUser(admin.Id);

        return (organization, project, admin);
    }
}
