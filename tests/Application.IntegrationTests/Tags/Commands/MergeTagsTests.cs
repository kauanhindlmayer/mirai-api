using Application.IntegrationTests.Infrastructure;
using Application.Tags.Commands.MergeTags;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Tags;
using Domain.Users;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Tags.Commands;

public class MergeTagsTests : BaseIntegrationTest
{
    public MergeTagsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task MergeTags_WhenWorkItemHasSourceTag_ShouldReassignItToTargetTag()
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

        var sourceTag = new Tag("frontend", null, "#ff0000", project.Id);
        var targetTag = new Tag("ui", null, "#00ff00", project.Id);
        var workItem = new WorkItem(project.Id, 1, "Title", WorkItemType.UserStory);
        _dbContext.Tags.AddRange(sourceTag, targetTag);
        _dbContext.WorkItems.Add(workItem);
        workItem.AddTag(sourceTag);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new MergeTagsCommand(project.Id, targetTag.Id, [sourceTag.Id]);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var reloadedWorkItem = await _dbContext.WorkItems
            .Include(wi => wi.Tags)
            .SingleAsync(wi => wi.Id == workItem.Id, TestContext.Current.CancellationToken);
        reloadedWorkItem.Tags.Should().ContainSingle(t => t.Id == targetTag.Id);

        var sourceTagStillExists = await _dbContext.Tags.AnyAsync(
            t => t.Id == sourceTag.Id,
            TestContext.Current.CancellationToken);
        sourceTagStillExists.Should().BeFalse();
    }

    [Fact]
    public async Task MergeTags_WhenWorkItemAlreadyHasTargetTag_ShouldNotDuplicateTheAssociation()
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

        var sourceTag = new Tag("frontend", null, "#ff0000", project.Id);
        var targetTag = new Tag("ui", null, "#00ff00", project.Id);
        var workItem = new WorkItem(project.Id, 1, "Title", WorkItemType.UserStory);
        _dbContext.Tags.AddRange(sourceTag, targetTag);
        _dbContext.WorkItems.Add(workItem);

        workItem.AddTag(sourceTag);
        workItem.AddTag(targetTag);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        var command = new MergeTagsCommand(project.Id, targetTag.Id, [sourceTag.Id]);

        _dbContext.ChangeTracker.Clear();

        // Act
        var result = await _sender.Send(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        var reloadedWorkItem = await _dbContext.WorkItems
            .Include(wi => wi.Tags)
            .SingleAsync(wi => wi.Id == workItem.Id, TestContext.Current.CancellationToken);
        reloadedWorkItem.Tags.Should().ContainSingle(t => t.Id == targetTag.Id);
    }
}
