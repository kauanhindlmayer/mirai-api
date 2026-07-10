using Application.IntegrationTests.Infrastructure;
using Application.Teams.Queries.GetTeamMembers;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Teams;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Teams.Queries;

public class GetTeamMembersTests : BaseIntegrationTest
{
    public GetTeamMembersTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetTeamMembers_WhenTeamHasMoreMembersThanPageSize_ShouldReturnFirstPage()
    {
        // Arrange
        var team = await CreateTeamWithMembersAsync(12);

        // Act
        var result = await _sender.Send(
            new GetTeamMembersQuery(team.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(10);
        result.Value.TotalCount.Should().Be(12);
        result.Value.HasNextPage.Should().BeTrue();
        result.Value.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetTeamMembers_WhenRequestingSecondPage_ShouldReturnRemainingMembers()
    {
        // Arrange
        var team = await CreateTeamWithMembersAsync(12);

        // Act
        var result = await _sender.Send(
            new GetTeamMembersQuery(team.Id, Page: 2, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(2);
        result.Value.HasNextPage.Should().BeFalse();
        result.Value.HasPreviousPage.Should().BeTrue();
    }

    private async Task<Team> CreateTeamWithMembersAsync(int memberCount)
    {
        await SeedCurrentUserAsync();
        var organization = new Organization(
            $"Organization {Guid.NewGuid()}",
            "Description");
        _dbContext.Organizations.Add(organization);

        var project = new Project(
            $"Project {Guid.NewGuid()}",
            "Description",
            organization.Id);
        _dbContext.Projects.Add(project);

        var team = new Team(project.Id, "Engineering", "Description");
        _dbContext.Teams.Add(team);

        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var users = Enumerable.Range(0, memberCount)
            .Select(i => new User(
                $"First{i}",
                $"Last{i}",
                $"member-{Guid.NewGuid()}@mirai.com"))
            .ToList();
        var teamMemberRole = await GetRoleAsync(SystemRoles.TeamMemberId);
        foreach (var user in users)
        {
            user.SetIdentityId(Guid.NewGuid().ToString());
            team.AddMember(user, teamMemberRole);
        }

        _dbContext.Users.AddRange(users);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(users[0].Id);

        return team;
    }
}
