using Application.IntegrationTests.Infrastructure;
using Application.WorkItems.Commands.AddComment;
using Application.WorkItems.Commands.AssignWorkItem;
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

public class WorkItemCommentNotificationTests : BaseIntegrationTest
{
    public WorkItemCommentNotificationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddComment_WhenUserIsMentioned_ShouldNotifyMentionedUser()
    {
        // Arrange
        var (author, _, workItem) = await CreateAssignedWorkItemAsync();
        var mentioned = await CreateProjectMemberAsync(workItem.ProjectId);
        SetCurrentUser(author.Id);

        var content = $"""Thanks <span data-type="mention" data-id="{mentioned.Id}">@Mentioned</span>""";

        // Act
        await _sender.Send(new AddCommentCommand(workItem.Id, content), TestContext.Current.CancellationToken);

        // Assert
        var notification = await GetSingleNotificationAsync(mentioned.Id, NotificationType.MentionedInWorkItemComment);
        notification.EntityId.Should().Be(workItem.Id);
    }

    [Fact]
    public async Task AddComment_OnAssignedWorkItem_ShouldNotifyAssignee()
    {
        // Arrange
        var (author, assignee, workItem) = await CreateAssignedWorkItemAsync();
        SetCurrentUser(author.Id);

        // Act
        await _sender.Send(
            new AddCommentCommand(workItem.Id, "Just a plain comment, no mention."),
            TestContext.Current.CancellationToken);

        // Assert
        var notification = await GetSingleNotificationAsync(assignee.Id, NotificationType.WorkItemCommentAdded);
        notification.EntityId.Should().Be(workItem.Id);
    }

    [Fact]
    public async Task AddComment_WhenAuthorMentionsThemselves_ShouldNotNotify()
    {
        // Arrange
        var (author, _, workItem) = await CreateAssignedWorkItemAsync();
        SetCurrentUser(author.Id);

        var content = $"""Note to self <span data-type="mention" data-id="{author.Id}">@Author</span>""";

        // Act
        await _sender.Send(new AddCommentCommand(workItem.Id, content), TestContext.Current.CancellationToken);

        // Assert
        await AssertNoNotificationAsync(author.Id, NotificationType.MentionedInWorkItemComment);
    }

    [Fact]
    public async Task AddComment_WhenAssigneeCommentsOnOwnWorkItem_ShouldNotNotifyThemselves()
    {
        // Arrange
        var (_, assignee, workItem) = await CreateAssignedWorkItemAsync();
        SetCurrentUser(assignee.Id);

        // Act
        await _sender.Send(
            new AddCommentCommand(workItem.Id, "Commenting on my own work item."),
            TestContext.Current.CancellationToken);

        // Assert
        await AssertNoNotificationAsync(assignee.Id, NotificationType.WorkItemCommentAdded);
    }

    [Fact]
    public async Task AddComment_WhenAssigneeIsAlsoMentioned_ShouldReceiveBothNotificationTypes()
    {
        // Arrange
        var (author, assignee, workItem) = await CreateAssignedWorkItemAsync();
        SetCurrentUser(author.Id);

        var content = $"""<span data-type="mention" data-id="{assignee.Id}">@Assignee</span> please check this""";

        // Act
        await _sender.Send(new AddCommentCommand(workItem.Id, content), TestContext.Current.CancellationToken);

        // Assert
        _dbContext.ChangeTracker.Clear();
        var notifications = await _dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.RecipientUserId == assignee.Id
                && (n.Type == NotificationType.MentionedInWorkItemComment
                    || n.Type == NotificationType.WorkItemCommentAdded))
            .ToListAsync(TestContext.Current.CancellationToken);
        notifications.Should().HaveCount(2);
        notifications.Should().Contain(n => n.Type == NotificationType.MentionedInWorkItemComment);
        notifications.Should().Contain(n => n.Type == NotificationType.WorkItemCommentAdded);
    }

    private async Task<Notification> GetSingleNotificationAsync(Guid recipientId, NotificationType type)
    {
        _dbContext.ChangeTracker.Clear();
        return await _dbContext.Notifications
            .AsNoTracking()
            .SingleAsync(
                n => n.RecipientUserId == recipientId && n.Type == type,
                TestContext.Current.CancellationToken);
    }

    private async Task AssertNoNotificationAsync(Guid recipientId, NotificationType type)
    {
        _dbContext.ChangeTracker.Clear();
        var exists = await _dbContext.Notifications
            .AsNoTracking()
            .AnyAsync(
                n => n.RecipientUserId == recipientId && n.Type == type,
                TestContext.Current.CancellationToken);
        exists.Should().BeFalse();
    }

    private async Task<User> CreateProjectMemberAsync(Guid projectId)
    {
        var project = await _dbContext.Projects.SingleAsync(p => p.Id == projectId, TestContext.Current.CancellationToken);
        var user = new User("Mentioned", "User", $"mentioned-{Guid.NewGuid()}@mirai.com");
        user.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        project.AddMember(user, await GetRoleAsync(SystemRoles.ProjectContributorId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return user;
    }

    private async Task<(User Author, User Assignee, WorkItem WorkItem)> CreateAssignedWorkItemAsync()
    {
        await SeedCurrentUserAsync();
        var organization = new Organization($"Organization {Guid.NewGuid()}", "Description");
        var project = new Project($"Project {Guid.NewGuid()}", "Description", organization.Id);
        var author = new User("Author", "User", $"author-{Guid.NewGuid()}@mirai.com");
        author.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Organizations.Add(organization);
        _dbContext.Projects.Add(project);
        _dbContext.Users.Add(author);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        project.AddMember(author, await GetRoleAsync(SystemRoles.ProjectAdminId));

        var workItem = new WorkItem(project.Id, 1, "Title", WorkItemType.UserStory);
        _dbContext.WorkItems.Add(workItem);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var assignee = new User("Jane", "Doe", $"jane-{Guid.NewGuid()}@mirai.com");
        assignee.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(assignee);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        project.AddMember(assignee, await GetRoleAsync(SystemRoles.ProjectContributorId));
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(author.Id);
        await _sender.Send(
            new AssignWorkItemCommand(workItem.Id, assignee.Id),
            TestContext.Current.CancellationToken);

        _dbContext.ChangeTracker.Clear();

        return (author, assignee, workItem);
    }
}
