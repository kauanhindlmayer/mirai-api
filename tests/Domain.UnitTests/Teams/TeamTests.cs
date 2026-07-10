using Domain.Authorization;
using Domain.Retrospectives;
using Domain.Sprints;
using Domain.Teams;
using Domain.Teams.Events;
using Domain.UnitTests.Boards;
using Domain.UnitTests.Infrastructure;
using Domain.UnitTests.Retrospectives;
using Domain.UnitTests.Sprints;
using Domain.UnitTests.Users;
using Domain.UnitTests.WorkItems;

namespace Domain.UnitTests.Teams;

public class TeamTests : BaseTest
{
    [Fact]
    public void Constructor_ShouldRaiseTeamCreatedDomainEvent()
    {
        // Arrange
        var team = TeamFactory.Create();

        // Assert
        var domainEvent = AssertDomainEventWasPublished<TeamCreatedDomainEvent>(team);
        domainEvent.Team.Should().Be(team);
    }

    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Act
        var team = TeamFactory.Create();

        // Assert
        team.ProjectId.Should().NotBeEmpty();
        team.Name.Should().Be(TeamFactory.Name);
        team.Description.Should().Be(TeamFactory.Description);
    }

    [Fact]
    public void AddMember_WhenUserAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        var user = UserFactory.Create();
        team.AddMember(user, SystemRoles.TeamMember);

        // Act
        var result = team.AddMember(user, SystemRoles.TeamMember);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(TeamErrors.UserAlreadyExists);
    }

    [Fact]
    public void AddMember_WhenUserDoesNotExist_ShouldAddMember()
    {
        // Arrange
        var team = TeamFactory.Create();
        var user = UserFactory.Create();

        // Act
        var result = team.AddMember(user, SystemRoles.TeamMember);

        // Assert
        result.IsError.Should().BeFalse();
        team.Members.Should().Contain(m => m.UserId == user.Id);
    }

    [Fact]
    public void AddMember_WhenRoleScopeDoesNotMatch_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        var user = UserFactory.Create();

        // Act
        var result = team.AddMember(user, SystemRoles.ProjectContributor);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(RoleErrors.ScopeMismatch);
    }

    [Fact]
    public void ChangeMemberRole_WhenDemotingLastAdmin_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        var admin = UserFactory.Create();
        team.AddMember(admin, SystemRoles.TeamAdmin);

        // Act
        var result = team.ChangeMemberRole(admin.Id, SystemRoles.TeamMember);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(TeamErrors.CannotRemoveLastAdmin);
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
    public void RemoveUser_WhenUserHasAssignedWorkItems_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        var user = UserFactory.Create();
        var workItem = WorkItemFactory.Create(assignedTeamId: team.Id);
        workItem.UpdateAssignment(user.Id);
        team.AddMember(user, SystemRoles.TeamMember);
        team.WorkItems.Add(workItem);

        // Act
        var result = team.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(TeamErrors.UserHasAssignedWorkItems);
    }

    [Fact]
    public void RemoveUser_WhenUserExists_ShouldRemoveUser()
    {
        // Arrange
        var team = TeamFactory.Create();
        var admin = UserFactory.Create();
        var user = UserFactory.Create(email: "member@example.com");
        team.AddMember(admin, SystemRoles.TeamAdmin);
        team.AddMember(user, SystemRoles.TeamMember);

        // Act
        var result = team.RemoveUser(user.Id);

        // Assert
        result.IsError.Should().BeFalse();
        team.Members.Should().NotContain(m => m.UserId == user.Id);
    }

    [Fact]
    public void RemoveUser_WhenRemovingLastAdmin_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        var admin = UserFactory.Create();
        team.AddMember(admin, SystemRoles.TeamAdmin);

        // Act
        var result = team.RemoveUser(admin.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(TeamErrors.CannotRemoveLastAdmin);
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

    [Fact]
    public void AddBoard_WhenBoardAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        var board = BoardFactory.Create();
        team.AddBoard(board);

        // Act
        var result = team.AddBoard(board);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(TeamErrors.BoardAlreadyExists);
    }

    [Fact]
    public void AddBoard_WhenBoardDoesNotExist_ShouldAddBoard()
    {
        // Arrange
        var team = TeamFactory.Create();
        var board = BoardFactory.Create();

        // Act
        var result = team.AddBoard(board);

        // Assert
        result.IsError.Should().BeFalse();
        team.Board.Should().Be(board);
    }
}