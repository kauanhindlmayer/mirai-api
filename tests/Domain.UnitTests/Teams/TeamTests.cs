using Domain.Retrospectives;
using Domain.Sprints;
using Domain.Teams;
using Domain.Teams.Events;
using Domain.UnitTests.Common;
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
        var team = TeamFactory.CreateTeam();

        // Assert
        var domainEvent = AssertDomainEventWasPublished<TeamCreatedDomainEvent>(team);
        domainEvent.Team.Should().Be(team);
    }

    [Fact]
    public void CreateTeam_ShouldSetProperties()
    {
        // Act
        var team = TeamFactory.CreateTeam();

        // Assert
        team.ProjectId.Should().NotBeEmpty();
        team.Name.Should().Be(TeamFactory.Name);
        team.Description.Should().Be(TeamFactory.Description);
    }

    [Fact]
    public void AddMember_WhenMemberAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.CreateTeam();
        var member = UserFactory.CreateUser();
        team.AddMember(member);

        // Act
        var result = team.AddMember(member);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(TeamErrors.MemberAlreadyExists);
    }

    [Fact]
    public void AddMember_WhenMemberDoesNotExist_ShouldAddMember()
    {
        // Arrange
        var team = TeamFactory.CreateTeam();
        var member = UserFactory.CreateUser();

        // Act
        var result = team.AddMember(member);

        // Assert
        result.IsError.Should().BeFalse();
        team.Members.Should().Contain(member);
    }

    [Fact]
    public void RemoveMember_WhenMemberDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.CreateTeam();
        var member = UserFactory.CreateUser();

        // Act
        var result = team.RemoveMember(member);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.Should().BeEquivalentTo(TeamErrors.MemberNotFound);
    }

    [Fact]
    public void RemoveMember_WhenMemberExists_ShouldRemoveMember()
    {
        // Arrange
        var team = TeamFactory.CreateTeam();
        var member = UserFactory.CreateUser();
        team.AddMember(member);

        // Act
        var result = team.RemoveMember(member);

        // Assert
        result.IsError.Should().BeFalse();
        team.Members.Should().NotContain(member);
    }

    [Fact]
    public void AddRetrospective_WhenRetrospectiveAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var team = TeamFactory.CreateTeam();
        var retrospective = RetrospectiveFactory.CreateRetrospective();
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
        var team = TeamFactory.CreateTeam();
        var retrospective = RetrospectiveFactory.CreateRetrospective();

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
        var team = TeamFactory.CreateTeam();
        var sprint = SprintFactory.CreateSprint();
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
        var team = TeamFactory.CreateTeam();
        var sprint = SprintFactory.CreateSprint();

        // Act
        var result = team.AddSprint(sprint);

        // Assert
        result.IsError.Should().BeFalse();
        team.Sprints.Should().Contain(sprint);
    }
}