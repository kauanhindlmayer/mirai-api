using Application.IntegrationTests.Infrastructure;
using Application.WorkItems.Queries.GetWorkItemHistory;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Tags;
using Domain.Users;
using Domain.WorkItems;
using Domain.WorkItems.Enums;
using FluentAssertions;

namespace Application.IntegrationTests.WorkItems.Queries;

public class GetWorkItemStructuralHistoryTests : BaseIntegrationTest
{
    public GetWorkItemStructuralHistoryTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddTag_ShouldRecordTagAddedInHistory()
    {
        // Arrange
        var (_, project, workItem) = await CreateWorkItemAsync();
        var tag = new Tag("frontend", null, "#ff0000", project.Id);
        _dbContext.Tags.Add(tag);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();
        var trackedWorkItem = await _dbContext.WorkItems.FindAsync(
            [workItem.Id],
            TestContext.Current.CancellationToken);
        var trackedTag = await _dbContext.Tags.FindAsync([tag.Id], TestContext.Current.CancellationToken);

        // Act
        trackedWorkItem!.AddTag(trackedTag!);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        var change = result.Value.Items.Single().Changes.Single(c => c.FieldName == "Tag");
        change.OldValue.Should().BeNull();
        change.NewValue.Should().Be("frontend");
    }

    [Fact]
    public async Task RemoveTag_ShouldRecordTagRemovedInHistory()
    {
        // Arrange
        var (_, project, workItem) = await CreateWorkItemAsync();
        var tag = new Tag("frontend", null, "#ff0000", project.Id);
        _dbContext.Tags.Add(tag);
        workItem.AddTag(tag);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();
        var trackedWorkItem = await _dbContext.WorkItems.FindAsync(
            [workItem.Id],
            TestContext.Current.CancellationToken);
        await _dbContext.Entry(trackedWorkItem!).Collection(w => w.Tags).LoadAsync(TestContext.Current.CancellationToken);

        // Act
        trackedWorkItem!.RemoveTag(tag.Name);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        var change = result.Value.Items[0].Changes.Single(c => c.FieldName == "Tag");
        change.OldValue.Should().Be("frontend");
        change.NewValue.Should().BeNull();
    }

    [Fact]
    public async Task AddAttachment_ShouldRecordAttachmentAddedInHistory()
    {
        // Arrange
        var (admin, _, workItem) = await CreateWorkItemAsync();

        _dbContext.ChangeTracker.Clear();
        var trackedWorkItem = await _dbContext.WorkItems.FindAsync(
            [workItem.Id],
            TestContext.Current.CancellationToken);

        // Act
        trackedWorkItem!.AddAttachment(new WorkItemAttachment(
            workItem.Id,
            "design.png",
            Guid.NewGuid(),
            "image/png",
            1024,
            admin.Id));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        var change = result.Value.Items.Single().Changes.Single(c => c.FieldName == "Attachment");
        change.OldValue.Should().BeNull();
        change.NewValue.Should().Be("design.png");
    }

    [Fact]
    public async Task AddLink_ShouldRecordLinkAddedInHistoryWithTargetWorkItemTitle()
    {
        // Arrange
        var (_, project, workItem) = await CreateWorkItemAsync();
        var targetWorkItem = new WorkItem(project.Id, 2, "Target Title", WorkItemType.Bug);
        _dbContext.WorkItems.Add(targetWorkItem);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();
        var trackedWorkItem = await _dbContext.WorkItems.FindAsync(
            [workItem.Id],
            TestContext.Current.CancellationToken);

        // Act
        trackedWorkItem!.AddLink(new WorkItemLink(workItem.Id, targetWorkItem.Id, WorkItemLinkType.Related));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        var change = result.Value.Items.Single().Changes.Single(c => c.FieldName == "Link");
        change.OldValue.Should().BeNull();
        change.NewValue.Should().Be("Related \"Target Title\"");
    }

    [Fact]
    public async Task AddPullRequestLink_ShouldRecordPullRequestAddedInHistory()
    {
        // Arrange
        var (_, _, workItem) = await CreateWorkItemAsync();

        _dbContext.ChangeTracker.Clear();
        var trackedWorkItem = await _dbContext.WorkItems.FindAsync(
            [workItem.Id],
            TestContext.Current.CancellationToken);

        // Act
        trackedWorkItem!.AddPullRequestLink(new WorkItemPullRequestLink(
            workItem.Id,
            pullRequestId: 123,
            pullRequestNumber: 42,
            title: "Fix login bug",
            htmlUrl: "https://github.com/org/repo/pull/42",
            state: PullRequestLinkState.Open,
            authorLogin: "octocat",
            source: PullRequestLinkSource.Manual));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(workItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        var change = result.Value.Items.Single().Changes.Single(c => c.FieldName == "Pull Request");
        change.OldValue.Should().BeNull();
        change.NewValue.Should().Be("#42 Fix login bug");
    }

    [Fact]
    public async Task CreateWorkItemWithInitialTag_ShouldNotRecordHistory()
    {
        // Arrange
        var (_, project, _) = await CreateWorkItemAsync();
        var tag = new Tag("backend", null, "#00ff00", project.Id);
        _dbContext.Tags.Add(tag);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();

        // Act
        var newWorkItem = new WorkItem(project.Id, 99, "Brand New", WorkItemType.UserStory);
        var trackedTag = await _dbContext.Tags.FindAsync([tag.Id], TestContext.Current.CancellationToken);
        newWorkItem.AddTag(trackedTag!);
        _dbContext.WorkItems.Add(newWorkItem);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _sender.Send(
            new GetWorkItemHistoryQuery(newWorkItem.Id, Page: 1, PageSize: 10),
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Items.Should().BeEmpty();
    }

    private async Task<(User Admin, Project Project, WorkItem WorkItem)> CreateWorkItemAsync()
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

        return (admin, project, workItem);
    }
}
