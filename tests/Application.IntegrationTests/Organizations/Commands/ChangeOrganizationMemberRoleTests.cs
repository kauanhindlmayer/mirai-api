using Application.IntegrationTests.Infrastructure;
using Application.Organizations.Commands.ChangeOrganizationMemberRole;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Organizations.Commands;

public class ChangeOrganizationMemberRoleTests : BaseIntegrationTest
{
    public ChangeOrganizationMemberRoleTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ChangeOrganizationMemberRole_WhenMemberExists_ShouldChangeRole()
    {
        // Arrange
        var (organization, member) = await SeedOrganizationWithMemberAsync();
        var command = new ChangeOrganizationMemberRoleCommand(
            organization.Id,
            member.Id,
            SystemRoles.OrganizationAdminId);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task ChangeOrganizationMemberRole_WhenRoleDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var (organization, member) = await SeedOrganizationWithMemberAsync();
        var command = new ChangeOrganizationMemberRoleCommand(
            organization.Id,
            member.Id,
            Guid.NewGuid());

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(RoleErrors.NotFound);
    }

    [Fact]
    public async Task ChangeOrganizationMemberRole_WhenDemotingLastOwner_ShouldReturnError()
    {
        // Arrange
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var owner = new User("Owner", "User", $"owner-{Guid.NewGuid()}@mirai.com");
        owner.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Users.Add(owner);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(owner, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(owner.Id);
        var command = new ChangeOrganizationMemberRoleCommand(
            organization.Id,
            owner.Id,
            SystemRoles.OrganizationAdminId);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(OrganizationErrors.CannotRemoveLastOwner);
    }

    private async Task<(Organization Organization, User Member)> SeedOrganizationWithMemberAsync()
    {
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var owner = new User("Owner", "User", $"owner-{Guid.NewGuid()}@mirai.com");
        var member = new User("Member", "User", $"member-{Guid.NewGuid()}@mirai.com");
        owner.SetIdentityId(Guid.NewGuid().ToString());
        member.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Users.AddRange(owner, member);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(owner, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        organization.AddMember(member, await GetRoleAsync(SystemRoles.OrganizationMemberId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(owner.Id);

        return (organization, member);
    }
}
