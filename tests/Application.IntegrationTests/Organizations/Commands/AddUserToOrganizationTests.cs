using Application.IntegrationTests.Infrastructure;
using Application.Organizations.Commands.AddUserToOrganization;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Organizations.Commands;

public class AddUserToOrganizationTests : BaseIntegrationTest
{
    public AddUserToOrganizationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddUserToOrganization_WhenUserExists_ShouldAddAsMember()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var user = new User("John", "Doe", $"john-{Guid.NewGuid()}@mirai.com");
        user.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(user, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(user.Id);
        var newUser = new User("Jane", "Smith", $"jane-{Guid.NewGuid()}@mirai.com");
        newUser.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new AddUserToOrganizationCommand(organization.Id, newUser.Email);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task AddUserToOrganization_WhenUserAlreadyExists_ShouldReturnError()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var owner = new User("John", "Doe", $"john-{Guid.NewGuid()}@mirai.com");
        var existingMember = new User("Jane", "Smith", $"jane-{Guid.NewGuid()}@mirai.com");
        owner.SetIdentityId(Guid.NewGuid().ToString());
        existingMember.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Users.AddRange(owner, existingMember);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        organization.AddMember(owner, await GetRoleAsync(SystemRoles.OrganizationOwnerId));
        organization.AddMember(existingMember, await GetRoleAsync(SystemRoles.OrganizationMemberId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(owner.Id);
        var command = new AddUserToOrganizationCommand(organization.Id, existingMember.Email);
        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(OrganizationErrors.UserAlreadyExists);
    }
}
