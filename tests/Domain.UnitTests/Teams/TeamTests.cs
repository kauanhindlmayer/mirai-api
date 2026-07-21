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
        result.FirstError.Code.Should().Be(SprintErrors.AlreadyExists.Code);
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
    public void AddSprint_WhenSprintOverlapsAnExistingSprint_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        team.AddSprint(SprintFactory.Create(
            name: "Sprint 1",
            startDate: new DateOnly(2026, 6, 1),
            endDate: new DateOnly(2026, 6, 14)));

        // Act
        var result = team.AddSprint(SprintFactory.Create(
            name: "Sprint 2",
            startDate: new DateOnly(2026, 6, 14),
            endDate: new DateOnly(2026, 6, 28)));

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(SprintErrors.Overlaps.Code);
    }

    [Fact]
    public void AddSprint_WhenSprintStartsTheDayAfterAnExistingSprintEnds_ShouldAddSprint()
    {
        // Arrange
        var team = TeamFactory.Create();
        team.AddSprint(SprintFactory.Create(
            name: "Sprint 1",
            startDate: new DateOnly(2026, 6, 1),
            endDate: new DateOnly(2026, 6, 14)));

        // Act
        var result = team.AddSprint(SprintFactory.Create(
            name: "Sprint 2",
            startDate: new DateOnly(2026, 6, 15),
            endDate: new DateOnly(2026, 6, 28)));

        // Assert
        result.IsError.Should().BeFalse();
        team.Sprints.Should().HaveCount(2);
    }

    [Fact]
    public void UpdateSprint_WhenSprintDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();

        // Act
        var result = team.UpdateSprint(
            Guid.NewGuid(),
            "Sprint 1",
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 14));

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(SprintErrors.NotFound);
    }

    [Fact]
    public void UpdateSprint_ShouldOverwriteNameAndDates()
    {
        // Arrange
        var team = TeamFactory.Create();
        var sprint = SprintFactory.Create();
        team.AddSprint(sprint);

        // Act
        var result = team.UpdateSprint(
            sprint.Id,
            "Sprint 42",
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 14));

        // Assert
        result.IsError.Should().BeFalse();
        sprint.Name.Should().Be("Sprint 42");
        sprint.StartDate.Should().Be(new DateOnly(2026, 6, 1));
        sprint.EndDate.Should().Be(new DateOnly(2026, 6, 14));
    }

    [Fact]
    public void UpdateSprint_WhenOnlyTheDatesChange_ShouldNotCollideWithItsOwnName()
    {
        // Arrange
        var team = TeamFactory.Create();
        var sprint = SprintFactory.Create(name: "Sprint 1");
        team.AddSprint(sprint);

        // Act
        var result = team.UpdateSprint(
            sprint.Id,
            "Sprint 1",
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 14));

        // Assert
        result.IsError.Should().BeFalse();
        sprint.StartDate.Should().Be(new DateOnly(2026, 6, 1));
    }

    [Fact]
    public void UpdateSprint_WhenOnlyTheNameChanges_ShouldNotOverlapItself()
    {
        // Arrange
        var team = TeamFactory.Create();
        var sprint = SprintFactory.Create(
            name: "Sprint 1",
            startDate: new DateOnly(2026, 6, 1),
            endDate: new DateOnly(2026, 6, 14));
        team.AddSprint(sprint);

        // Act
        var result = team.UpdateSprint(
            sprint.Id,
            "Sprint 42",
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 14));

        // Assert
        result.IsError.Should().BeFalse();
        sprint.Name.Should().Be("Sprint 42");
    }

    [Fact]
    public void UpdateSprint_WhenNameIsTakenByAnotherSprint_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        team.AddSprint(SprintFactory.Create(
            name: "Sprint 1",
            startDate: new DateOnly(2026, 6, 1),
            endDate: new DateOnly(2026, 6, 14)));
        var sprint = SprintFactory.Create(
            name: "Sprint 2",
            startDate: new DateOnly(2026, 6, 15),
            endDate: new DateOnly(2026, 6, 28));
        team.AddSprint(sprint);

        // Act
        var result = team.UpdateSprint(
            sprint.Id,
            "Sprint 1",
            sprint.StartDate,
            sprint.EndDate);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(SprintErrors.AlreadyExists.Code);
    }

    [Fact]
    public void UpdateSprint_WhenDatesOverlapAnotherSprint_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        team.AddSprint(SprintFactory.Create(
            name: "Sprint 1",
            startDate: new DateOnly(2026, 6, 1),
            endDate: new DateOnly(2026, 6, 14)));
        var sprint = SprintFactory.Create(
            name: "Sprint 2",
            startDate: new DateOnly(2026, 6, 15),
            endDate: new DateOnly(2026, 6, 28));
        team.AddSprint(sprint);

        // Act
        var result = team.UpdateSprint(
            sprint.Id,
            "Sprint 2",
            new DateOnly(2026, 6, 10),
            new DateOnly(2026, 6, 28));

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(SprintErrors.Overlaps.Code);
    }

    [Fact]
    public void UpdateSprint_WhenDatesOverlap_ShouldNameTheSprintTheyOverlap()
    {
        // Arrange
        var team = TeamFactory.Create();
        team.AddSprint(SprintFactory.Create(
            name: "Hardening",
            startDate: new DateOnly(2026, 6, 1),
            endDate: new DateOnly(2026, 6, 14)));
        var sprint = SprintFactory.Create(
            name: "Sprint 2",
            startDate: new DateOnly(2026, 6, 15),
            endDate: new DateOnly(2026, 6, 28));
        team.AddSprint(sprint);

        // Act
        var result = team.UpdateSprint(
            sprint.Id,
            "Sprint 2",
            new DateOnly(2026, 6, 10),
            new DateOnly(2026, 6, 28));

        // Assert
        result.FirstError.Description.Should().Contain("Hardening");
    }

    [Fact]
    public void UpdateSprint_WhenNameIsTaken_ShouldNameTheSprintThatHasIt()
    {
        // Arrange
        var team = TeamFactory.Create();
        team.AddSprint(SprintFactory.Create(
            name: "Hardening",
            startDate: new DateOnly(2026, 6, 1),
            endDate: new DateOnly(2026, 6, 14)));
        var sprint = SprintFactory.Create(
            name: "Sprint 2",
            startDate: new DateOnly(2026, 6, 15),
            endDate: new DateOnly(2026, 6, 28));
        team.AddSprint(sprint);

        // Act
        var result = team.UpdateSprint(
            sprint.Id,
            "Hardening",
            sprint.StartDate,
            sprint.EndDate);

        // Assert
        result.FirstError.Description.Should().Contain("Hardening");
    }

    [Fact]
    public void StartSprint_WhenSprintDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();

        // Act
        var result = team.StartSprint(Guid.NewGuid());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(SprintErrors.NotFound);
    }

    [Fact]
    public void StartSprint_ShouldMakeTheSprintActive()
    {
        // Arrange
        var team = TeamFactory.Create();
        var sprint = SprintFactory.Create();
        team.AddSprint(sprint);

        // Act
        var result = team.StartSprint(sprint.Id);

        // Assert
        result.IsError.Should().BeFalse();
        sprint.Status.Should().Be(SprintStatus.Active);
        sprint.StartedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void StartSprint_WhenSprintHasNoWorkItems_ShouldStillStart()
    {
        // Arrange
        var team = TeamFactory.Create();
        var sprint = SprintFactory.Create();
        team.AddSprint(sprint);
        sprint.WorkItems.Should().BeEmpty();

        // Act
        var result = team.StartSprint(sprint.Id);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void StartSprint_WhenAnotherSprintIsActive_ShouldReturnErrorNamingIt()
    {
        // Arrange
        var team = TeamFactory.Create();
        var active = SprintFactory.Create(
            name: "Hardening",
            startDate: new DateOnly(2026, 6, 1),
            endDate: new DateOnly(2026, 6, 14));
        team.AddSprint(active);
        team.StartSprint(active.Id);

        var next = SprintFactory.Create(
            name: "Sprint 2",
            startDate: new DateOnly(2026, 6, 15),
            endDate: new DateOnly(2026, 6, 28));
        team.AddSprint(next);

        // Act
        var result = team.StartSprint(next.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(SprintErrors.TeamAlreadyHasActiveSprint("any").Code);
        result.FirstError.Description.Should().Contain("Hardening");
        next.Status.Should().Be(SprintStatus.Planned);
    }

    [Fact]
    public void StartSprint_WhenSprintIsAlreadyActive_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();
        var sprint = SprintFactory.Create();
        team.AddSprint(sprint);
        team.StartSprint(sprint.Id);

        // Act
        var result = team.StartSprint(sprint.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(SprintErrors.NotPlanned.Code);
    }

    [Fact]
    public void DeleteSprint_WhenSprintDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.Create();

        // Act
        var result = team.DeleteSprint(Guid.NewGuid());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(SprintErrors.NotFound);
    }

    [Fact]
    public void DeleteSprint_ShouldRemoveSprintAndReturnItsWorkItemsToTheBacklog()
    {
        // Arrange
        var team = TeamFactory.Create();
        var sprint = SprintFactory.Create();
        var workItem = WorkItemFactory.Create();
        sprint.AddWorkItem(workItem);
        team.AddSprint(sprint);

        // Act
        var result = team.DeleteSprint(sprint.Id);

        // Assert
        result.IsError.Should().BeFalse();
        team.Sprints.Should().BeEmpty();
        workItem.SprintId.Should().BeNull();
    }

    [Fact]
    public void DeleteSprint_ShouldFreeItsNameAndDatesForReuse()
    {
        // Arrange
        var team = TeamFactory.Create();
        var sprint = SprintFactory.Create();
        team.AddSprint(sprint);
        team.DeleteSprint(sprint.Id);

        // Act
        var result = team.AddSprint(SprintFactory.Create(
            name: sprint.Name,
            startDate: sprint.StartDate,
            endDate: sprint.EndDate));

        // Assert
        result.IsError.Should().BeFalse();
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