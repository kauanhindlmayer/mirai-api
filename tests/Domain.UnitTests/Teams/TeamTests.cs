using Domain.Retrospectives;
using Domain.Sprints;
using Domain.Teams;
using Domain.Teams.Events;
using Domain.UnitTests.Infrastructure;
using Domain.UnitTests.Retrospectives;
using Domain.UnitTests.Sprints;
using Domain.UnitTests.Users;

namespace Domain.UnitTests.Teams;

public class TeamTests : BaseTest
{
    [Fact]
    public void CreateTeam_ShouldRaiseTeamCreatedDomainEvent()
    {
        // Arrange
        var team = TeamFactory.Create();

        // Assert
        var domainEvent = AssertDomainEventWasPublished<TeamCreatedDomainEvent>(team);
        domainEvent.Team.Should().Be(team);
    }

    [Fact]
    public void CreateTeam_ShouldSetProperties()
    {
        // Act
        var team = TeamFactory.Create();

        // Assert
        team.ProjectId.Should().NotBeEmpty();
        team.Name.Should().Be(TeamFactory.Name);
        team.Description.Should().Be(TeamFactory.Description);
    }

    [Fact]
    public void AddUser_WhenUserAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        var user = UserFactory.Create();
        team.AddUser(user);

        // Act
        var result = team.AddUser(user);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(TeamErrors.UserAlreadyExists);
    }

    [Fact]
    public void AddUser_WhenUserDoesNotExist_ShouldAddUser()
    {
        // Arrange
        var team = TeamFactory.Create();
        var user = UserFactory.Create();

        // Act
        var result = team.AddUser(user);

        // Assert
        result.IsError.Should().BeFalse();
        team.Users.Should().Contain(user);
    }

    [Fact]
    public void RemoveUser_WhenUserDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        var user = UserFactory.Create();

        // Act
        var result = team.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(TeamErrors.UserNotFound);
    }

    [Fact]
    public void RemoveUser_WhenUserExists_ShouldRemoveUser()
    {
        // Arrange
        var team = TeamFactory.Create();
        var user = UserFactory.Create();
        team.AddUser(user);

        // Act
        var result = team.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeFalse();
        team.Users.Should().NotContain(user);
    }

    [Fact]
    public void AddRetrospective_WhenRetrospectiveAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        var retrospective = RetrospectiveFactory.Create();
        team.AddRetrospective(retrospective);

        // Act
        var result = team.AddRetrospective(retrospective);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(RetrospectiveErrors.AlreadyExists);
    }

    [Fact]
    public void AddRetrospective_WhenRetrospectiveDoesNotExist_ShouldAddRetrospective()
    {
        // Arrange
        var team = TeamFactory.Create();
        var retrospective = RetrospectiveFactory.Create();

        // Act
        var result = team.AddRetrospective(retrospective);

        // Assert
        result.IsError.Should().BeFalse();
        team.Retrospectives.Should().Contain(retrospective);
    }

    [Fact]
    public void AddSprint_WhenSprintAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        var sprint = SprintFactory.Create();
        team.AddSprint(sprint);

        // Act
        var result = team.AddSprint(sprint);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(SprintErrors.AlreadyExists);
    }

    [Fact]
    public void AddSprint_WhenSprintDoesNotExist_ShouldAddSprint()
    {
        // Arrange
        var team = TeamFactory.Create();
        var sprint = SprintFactory.Create();

        // Act
        var result = team.AddSprint(sprint);

        // Assert
        result.IsError.Should().BeFalse();
        team.Sprints.Should().Contain(sprint);
    }
}