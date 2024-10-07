using Domain.Teams;
using TestCommon.Teams;

namespace Domain.UnitTests.Teams;

public class TeamTests
{
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
        result.Errors.First().Should().BeEquivalentTo(TeamErrors.MemberAlreadyExists);
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
        result.Errors.First().Should().BeEquivalentTo(TeamErrors.MemberNotFound);
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
}