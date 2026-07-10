using Application.IntegrationTests.Infrastructure;
using Application.Projects.Commands.CreateProject;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Users;
using FluentAssertions;

namespace Application.IntegrationTests.Projects.Commands;

public class CreateProjectTests : BaseIntegrationTest
{
    public CreateProjectTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateProject_WhenValidCommand_ShouldCreateProjectWithCallerAsAdmin()
    {
        // Arrange
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var user = new User("Test", "User", $"user-{Guid.NewGuid()}@mirai.com");
        user.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var ownerRole = await GetRoleAsync(SystemRoles.OrganizationOwnerId);
        organization.AddMember(user, ownerRole);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(user.Id);

        var command = new CreateProjectCommand(
            $"Project {Guid.NewGuid()}",
            "Description",
            organization.Id);

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var project = await _dbContext.Projects.FindAsync([result.Value], TestContext.Current.CancellationToken);
        await _dbContext.Entry(project!).Collection(p => p.Members).LoadAsync(TestContext.Current.CancellationToken);
        project!.Members.Should().Contain(m => m.UserId == user.Id && m.RoleId == SystemRoles.ProjectAdminId);
    }
}
