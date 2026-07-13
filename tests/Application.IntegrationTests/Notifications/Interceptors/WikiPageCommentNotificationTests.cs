using Application.IntegrationTests.Infrastructure;
using Application.WikiPages.Commands.AddComment;
using Domain.Authorization;
using Domain.Notifications;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Domain.WikiPages;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Notifications.Interceptors;

public class WikiPageCommentNotificationTests : BaseIntegrationTest
{
    public WikiPageCommentNotificationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddComment_WhenUserIsMentioned_ShouldNotifyMentionedUser()
    {
        // Arrange
        var (author, wikiPage) = await CreateWikiPageAsync();
        var mentioned = new User("Mentioned", "User", $"mentioned-{Guid.NewGuid()}@mirai.com");
        mentioned.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(mentioned);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(author.Id);
        var content = $"""See <span data-type="mention" data-id="{mentioned.Id}">@Mentioned</span></p>""";

        // Act
        await _sender.Send(new AddCommentCommand(wikiPage.Id, content), TestContext.Current.CancellationToken);

        // Assert
        _dbContext.ChangeTracker.Clear();
        var notification = await _dbContext.Notifications
            .AsNoTracking()
            .SingleAsync(
                n => n.RecipientUserId == mentioned.Id && n.Type == NotificationType.MentionedInWikiPageComment,
                TestContext.Current.CancellationToken);
        notification.EntityId.Should().Be(wikiPage.Id);
    }

    [Fact]
    public async Task AddComment_WhenAuthorMentionsThemselves_ShouldNotNotify()
    {
        // Arrange
        var (author, wikiPage) = await CreateWikiPageAsync();
        SetCurrentUser(author.Id);
        var content = $"""Note <span data-type="mention" data-id="{author.Id}">@Author</span>""";

        // Act
        await _sender.Send(new AddCommentCommand(wikiPage.Id, content), TestContext.Current.CancellationToken);

        // Assert
        _dbContext.ChangeTracker.Clear();
        var exists = await _dbContext.Notifications
            .AsNoTracking()
            .AnyAsync(
                n => n.RecipientUserId == author.Id && n.Type == NotificationType.MentionedInWikiPageComment,
                TestContext.Current.CancellationToken);
        exists.Should().BeFalse();
    }

    private async Task<(User Author, WikiPage WikiPage)> CreateWikiPageAsync()
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

        var wikiPage = new WikiPage(project.Id, "Home", "Welcome", author.Id);
        _dbContext.WikiPages.Add(wikiPage);
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        SetCurrentUser(author.Id);
        _dbContext.ChangeTracker.Clear();

        return (author, wikiPage);
    }
}
