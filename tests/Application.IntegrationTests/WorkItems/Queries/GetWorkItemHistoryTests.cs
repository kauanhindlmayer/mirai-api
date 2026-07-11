using Application.IntegrationTests.Infrastructure;
using Application.WorkItems.Commands.AssignWorkItem;
using Application.WorkItems.Commands.UnassignWorkItem;
using Application.WorkItems.Commands.UpdateWorkItem;
using Application.WorkItems.Queries.GetWorkItemHistory;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.WorkItems.Queries;

public class GetWorkItemHistoryTests : BaseIntegrationTest
{
    public GetWorkItemHistoryTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task UpdateWorkItem_WhenStatusChanges_ShouldRecordChangeInHistory()
    {
        // Arrange
        var (admin, workItem) = await CreateWorkItemAsync();

        _dbContext.ChangeTracker.Clear();

        // Act
        await _sender.Send(
            new UpdateWorkItemCommand(
                workItem.Id,
                Type: null,
                Title: null,
                Description: null,
                AcceptanceCriteria: null,
                Status: WorkItemStatus.Active,
                AssignedTeamId: null,
                SprintId: null,
                ParentWorkItemId: null,
                Planning: null,
                Classification: null),
            TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(1);
        var changeSet = result.Value.Items.Single();
        changeSet.ChangedBy.Should().NotBeNull();
        changeSet.ChangedBy!.Id.Should().Be(admin.Id);
        changeSet.Changes.Should().ContainSingle(c =>
            c.FieldName == "Status" && c.OldValue == "New" && c.NewValue == "Active");
    }

    [Fact]
    public async Task AssignWorkItem_WhenAssigneeChanges_ShouldRecordDisplayNameNotRawId()
    {
        // Arrange
        var (_, workItem) = await CreateWorkItemAsync();

        var assignee = new User("Jane", "Doe", $"jane-{Guid.NewGuid()}@mirai.com");
        assignee.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(assignee);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();

        // Act
        await _sender.Send(
            new AssignWorkItemCommand(workItem.Id, assignee.Id),
            TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        var change = result.Value.Items.Single().Changes.Single(c => c.FieldName == "Assignee");
        change.OldValue.Should().BeNull();
        change.NewValue.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task UpdateWorkItem_WhenAssigneeIsNotTouched_ShouldNotClearAssignee()
    {
        // Arrange
        var (_, workItem) = await CreateWorkItemAsync();

        var assignee = new User("Jane", "Doe", $"jane-{Guid.NewGuid()}@mirai.com");
        assignee.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(assignee);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await _sender.Send(
            new AssignWorkItemCommand(workItem.Id, assignee.Id),
            TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();

        // Act
        await _sender.Send(
            new UpdateWorkItemCommand(
                workItem.Id,
                Type: null,
                Title: null,
                Description: null,
                AcceptanceCriteria: null,
                Status: WorkItemStatus.Active,
                AssignedTeamId: null,
                SprintId: null,
                ParentWorkItemId: null,
                Planning: null,
                Classification: null),
            TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        var latestChangeSet = result.Value.Items[0];
        latestChangeSet.Changes.Should().ContainSingle(c => c.FieldName == "Status");
        latestChangeSet.Changes.Should().NotContain(c => c.FieldName == "Assignee");

        var updatedWorkItem = await _dbContext.WorkItems.FindAsync(
            [workItem.Id],
            TestContext.Current.CancellationToken);
        updatedWorkItem!.AssigneeId.Should().Be(assignee.Id);
    }

    [Fact]
    public async Task UnassignWorkItem_WhenWorkItemIsAssigned_ShouldRecordChangeToNull()
    {
        // Arrange
        var (_, workItem) = await CreateWorkItemAsync();

        var assignee = new User("Jane", "Doe", $"jane-{Guid.NewGuid()}@mirai.com");
        assignee.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(assignee);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await _sender.Send(
            new AssignWorkItemCommand(workItem.Id, assignee.Id),
            TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();

        // Act
        await _sender.Send(
            new UnassignWorkItemCommand(workItem.Id),
            TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        var change = result.Value.Items[0].Changes.Single(c => c.FieldName == "Assignee");
        change.OldValue.Should().Be("Jane Doe");
        change.NewValue.Should().BeNull();
    }

    [Fact]
    public async Task AssignWorkItem_WhenAssigneeIsLaterRenamed_ShouldKeepOriginalDisplayNameInHistory()
    {
        // Arrange
        var (_, workItem) = await CreateWorkItemAsync();

        var assignee = new User("Jane", "Doe", $"jane-{Guid.NewGuid()}@mirai.com");
        assignee.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(assignee);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();

        await _sender.Send(
            new AssignWorkItemCommand(workItem.Id, assignee.Id),
            TestContext.Current.CancellationToken);

        // Act
        assignee.UpdateProfile("Janet", "Smith");
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        var change = result.Value.Items.Single().Changes.Single(c => c.FieldName == "Assignee");
        change.NewValue.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task UpdateWorkItem_WhenMultipleFieldsChangeInOneSave_ShouldGroupIntoSingleChangeSet()
    {
        // Arrange
        var (_, workItem) = await CreateWorkItemAsync();

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

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.Single().Changes.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateWorkItem_WhenFieldIsUnchanged_ShouldNotRecordChange()
    {
        // Arrange
        var (_, workItem) = await CreateWorkItemAsync();

        _dbContext.ChangeTracker.Clear();

        // Act
        await _sender.Send(
            new UpdateWorkItemCommand(
                workItem.Id,
                Type: null,
                Title: workItem.Title,
                Description: null,
                AcceptanceCriteria: null,
                Status: WorkItemStatus.New,
                AssignedTeamId: null,
                SprintId: null,
                ParentWorkItemId: null,
                Planning: null,
                Classification: null),
            TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetWorkItemHistory_WhenMultipleChangeSetsExist_ShouldReturnNewestFirstPaginated()
    {
        // Arrange
        var (_, workItem) = await CreateWorkItemAsync();

        foreach (var status in new[] { WorkItemStatus.Active, WorkItemStatus.Resolved, WorkItemStatus.Closed })
        {
            _dbContext.ChangeTracker.Clear();
            await _sender.Send(
                new UpdateWorkItemCommand(
                    workItem.Id,
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

        // Act
        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 2),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.TotalCount.Should().Be(3);
        result.Value.Items.Should().HaveCount(2);
        result.Value.Items[0].Changes.Single().NewValue.Should().Be("Closed");
        result.Value.Items[1].Changes.Single().NewValue.Should().Be("Resolved");
    }

    private async Task<(User Admin, WorkItem WorkItem)> CreateWorkItemAsync()
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
