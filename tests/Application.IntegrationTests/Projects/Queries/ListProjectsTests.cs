using Application.IntegrationTests.Infrastructure;
using Application.Projects.Queries.ListProjects;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Projects.Queries;

public class ListProjectsTests : BaseIntegrationTest
{
    public ListProjectsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ListProjects_WhenCallerIsOrganizationOwner_ShouldReturnProjectsTheyAreNotDirectlyAMemberOf()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var owner = new User("Owner", "User", $"owner-{Guid.NewGuid()}@mirai.com");
        owner.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Users.Add(owner);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // The owner is only added at the organization scope, never directly to the project.
        organization.AddMember(owner, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(owner.Id);
        var query = new ListProjectsQuery(organization.Id);

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Contain(p => p.Id == project.Id);
    }

    [Fact]
    public async Task ListProjects_WhenCallerIsOrganizationMemberNotInProject_ShouldNotReturnThatProject()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var member = new User("Member", "User", $"member-{Guid.NewGuid()}@mirai.com");
        member.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Users.Add(member);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(member, await GetRoleAsync(SystemRoles.OrganizationMemberId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(member.Id);
        var query = new ListProjectsQuery(organization.Id);

        // Act
        var result = await _sender.Send(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotContain(p => p.Id == project.Id);
    }
}
