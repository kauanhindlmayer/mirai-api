using Application.IntegrationTests.Infrastructure;
using Application.WorkItems.Commands.LinkWorkItems;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.WorkItems.Commands;

public class LinkWorkItemsTests : BaseIntegrationTest
{
    public LinkWorkItemsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task LinkWorkItems_WhenLinkAlreadyExists_ShouldReturnError()
    {
        // Arrange
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var admin = new User("Admin", "User", $"admin-{Guid.NewGuid()}@mirai.com");
        admin.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        project.AddMember(admin, await GetRoleAsync(SystemRoles.ProjectAdminId));

        var sourceWorkItem = new WorkItem(project.Id, 1, "Source", WorkItemType.UserStory);
        var targetWorkItem = new WorkItem(project.Id, 2, "Target", WorkItemType.Bug);
        _dbContext.WorkItems.AddRange(sourceWorkItem, targetWorkItem);
        var existingLink = new WorkItemLink(
            sourceWorkItem.Id,
            targetWorkItem.Id,
            WorkItemLinkType.Related,
            null);
        sourceWorkItem.AddLink(existingLink);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new LinkWorkItemsCommand(
            sourceWorkItem.Id,
            targetWorkItem.Id,
            WorkItemLinkType.Related,
            "Another comment");

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WorkItemErrors.LinkAlreadyExists);
    }
}
