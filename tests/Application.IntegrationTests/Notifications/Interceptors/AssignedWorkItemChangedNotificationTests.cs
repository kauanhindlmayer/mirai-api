using Application.IntegrationTests.Infrastructure;
using Application.WorkItems.Commands.AssignWorkItem;
using Application.WorkItems.Commands.UpdateWorkItem;
using Domain.Authorization;
using Domain.Notifications;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Notifications.Interceptors;

public class AssignedWorkItemChangedNotificationTests : BaseIntegrationTest
{
    public AssignedWorkItemChangedNotificationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task UpdateWorkItem_WhenAssignedWorkItemFieldChanges_ShouldNotifyAssignee()
    {
        // Arrange
        var (admin, assignee, workItem) = await CreateAssignedWorkItemAsync();
        SetCurrentUser(admin.Id);
        _dbContext.ChangeTracker.Clear();

        // Act
        await SendUpdateStatusAsync(workItem.Id, WorkItemStatus.Active);

        // Assert
        var notification = await GetSingleNotificationAsync(assignee.Id);
        notification.Type.Should().Be(NotificationType.AssignedWorkItemChanged);
        notification.EntityId.Should().Be(workItem.Id);
        notification.Message.Should().Contain(admin.FullName).And.Contain(workItem.Title);
    }

    [Fact]
    public async Task UpdateWorkItem_WhenMultipleFieldsChangeInOneSave_ShouldCreateOneNotification()
    {
        // Arrange
        var (admin, assignee, workItem) = await CreateAssignedWorkItemAsync();
        SetCurrentUser(admin.Id);
        _dbContext.ChangeTracker.Clear();

        // Act
        await _sender.Send(
            new UpdateWorkItemCommand(
                workItem.Id,
                Type: null,
                Title: "Updated Title",
                Description: null,
                AcceptanceCriteria: null,
                Status: WorkItemStatus.Active,
                AssignedTeamId: null,
                SprintId: null,
                ParentWorkItemId: null,
                Planning: null,
                Classification: null),
            TestContext.Current.CancellationToken);

        // Assert
        var notification = await GetSingleNotificationAsync(assignee.Id);
        notification.Message.Should().Contain("Title").And.Contain("Status");
    }

    [Fact]
    public async Task AssignWorkItem_WhenReassignedInSameSaveAsAnotherFieldChange_ShouldNotifyNewAssigneeNotOutgoingOne()
    {
        // Arrange - guards against reading AssigneeId from the database (which still
        // holds the pre-save value at interceptor time) instead of the tracked entity.
        var (admin, outgoingAssignee, workItem) = await CreateAssignedWorkItemAsync();
        var newAssignee = new User("New", "Assignee", $"new-{Guid.NewGuid()}@mirai.com");
        newAssignee.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(newAssignee);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);
        _dbContext.ChangeTracker.Clear();

        // Act
        await _sender.Send(
            new AssignWorkItemCommand(workItem.Id, newAssignee.Id),
            TestContext.Current.CancellationToken);

        // Assert
        await AssertNoNotificationAsync(outgoingAssignee.Id);
        var notification = await GetSingleNotificationAsync(newAssignee.Id);
        notification.EntityId.Should().Be(workItem.Id);
    }

    [Fact]
    public async Task UpdateWorkItem_WhenActorIsAssignee_ShouldNotNotify()
    {
        // Arrange
        var (_, assignee, workItem) = await CreateAssignedWorkItemAsync();
        SetCurrentUser(assignee.Id);
        _dbContext.ChangeTracker.Clear();

        // Act
        await SendUpdateStatusAsync(workItem.Id, WorkItemStatus.Active);

        // Assert
        await AssertNoNotificationAsync(assignee.Id);
    }

    [Fact]
    public async Task UpdateWorkItem_WhenWorkItemHasNoAssignee_ShouldNotNotify()
    {
        // Arrange
        var (admin, workItem) = await CreateUnassignedWorkItemAsync();
        SetCurrentUser(admin.Id);
        _dbContext.ChangeTracker.Clear();

        // Act
        await SendUpdateStatusAsync(workItem.Id, WorkItemStatus.Active);

        // Assert
        var anyNotification = await _dbContext.Notifications
            .AsNoTracking()
            .AnyAsync(n => n.Type == NotificationType.AssignedWorkItemChanged, TestContext.Current.CancellationToken);
        anyNotification.Should().BeFalse();
    }

    [Fact]
    public async Task SaveChanges_WhenChangeSetHasNoChangedByUser_ShouldNotNotifyAssignee()
    {
        // Arrange - a WorkItemChangeSet with a null actor mirrors what a background
        // job/integration would stage (see docs/adr/0007-workitem-change-history.md);
        // constructed directly since no job currently produces one through the real pipeline.
        var (_, assignee, workItem) = await CreateAssignedWorkItemAsync();
        _dbContext.ChangeTracker.Clear();

        var changeSet = new WorkItemChangeSet(workItem.Id, changedByUserId: null, systemActor: "GitHub Integration");
        changeSet.AddChange("Status", "New", "Active");
        _dbContext.Set<WorkItemChangeSet>().Add(changeSet);

        // Act
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        await AssertNoNotificationAsync(assignee.Id);
    }

    private Task SendUpdateStatusAsync(Guid workItemId, WorkItemStatus status)
    {
        return _sender.Send(
            new UpdateWorkItemCommand(
                workItemId,
                Type: null,
                Title: null,
                Description: null,
                AcceptanceCriteria: null,
                Status: status,
                AssignedTeamId: null,
                SprintId: null,
                ParentWorkItemId: null,
                Planning: null,
                Classification: null),
            TestContext.Current.CancellationToken);
    }

    private async Task<Notification> GetSingleNotificationAsync(Guid recipientId)
    {
        _dbContext.ChangeTracker.Clear();
        return await _dbContext.Notifications
            .AsNoTracking()
            .SingleAsync(
                n => n.RecipientUserId == recipientId && n.Type == NotificationType.AssignedWorkItemChanged,
                TestContext.Current.CancellationToken);
    }

    private async Task AssertNoNotificationAsync(Guid recipientId)
    {
        _dbContext.ChangeTracker.Clear();
        var exists = await _dbContext.Notifications
            .AsNoTracking()
            .AnyAsync(
                n => n.RecipientUserId == recipientId && n.Type == NotificationType.AssignedWorkItemChanged,
                TestContext.Current.CancellationToken);
        exists.Should().BeFalse();
    }

    private async Task<(User Admin, User Assignee, WorkItem WorkItem)> CreateAssignedWorkItemAsync()
    {
        var (admin, workItem) = await CreateUnassignedWorkItemAsync();

        var assignee = new User("Jane", "Doe", $"jane-{Guid.NewGuid()}@mirai.com");
        assignee.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(assignee);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await _sender.Send(
            new AssignWorkItemCommand(workItem.Id, assignee.Id),
            TestContext.Current.CancellationToken);

        return (admin, assignee, workItem);
    }

    private async Task<(User Admin, WorkItem WorkItem)> CreateUnassignedWorkItemAsync()
    {
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

        var workItem = new WorkItem(project.Id, 1, "Title", WorkItemType.UserStory);
        _dbContext.WorkItems.Add(workItem);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(admin.Id);

        return (admin, workItem);
    }
}
