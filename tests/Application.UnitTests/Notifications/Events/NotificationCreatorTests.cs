using Application.Notifications.Events;
using Domain.Notifications;
using Domain.Shared;
using Microsoft.Extensions.Logging.Abstractions;

namespace Application.UnitTests.Notifications.Events;

public class NotificationCreatorTests
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationPreferenceRepository _notificationPreferenceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly NotificationCreator _notificationCreator;

    public NotificationCreatorTests()
    {
        _notificationRepository = Substitute.For<INotificationRepository>();
        _notificationPreferenceRepository = Substitute.For<INotificationPreferenceRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _notificationCreator = new NotificationCreator(
            _notificationRepository,
            _notificationPreferenceRepository,
            _unitOfWork,
            NullLogger<NotificationCreator>.Instance);
    }

    [Fact]
    public async Task CreateAsync_WhenRecipientIsActor_ShouldNotCreateNotification()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        await _notificationCreator.CreateAsync(
            userId,
            userId,
            NotificationType.AddedToProject,
            Guid.NewGuid(),
            "You were added to the project \"Mirai\".",
            TestContext.Current.CancellationToken);

        // Assert
        await _notificationRepository.DidNotReceive().AddAsync(
            Arg.Any<Notification>(),
            Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WhenRecipientIsNotActor_ShouldCreateAndSaveNotification()
    {
        // Arrange
        var recipientId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        const string message = "You were added to the project \"Mirai\".";

        // Act
        await _notificationCreator.CreateAsync(
            recipientId,
            actorId,
            NotificationType.AddedToProject,
            entityId,
            message,
            TestContext.Current.CancellationToken);

        // Assert
        await _notificationRepository.Received(1).AddAsync(
            Arg.Is<Notification>(n =>
                n.RecipientUserId == recipientId &&
                n.Type == NotificationType.AddedToProject &&
                n.EntityId == entityId &&
                n.Message == message),
            TestContext.Current.CancellationToken);
        await _unitOfWork.Received(1).SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CreateAsync_WhenSaveThrows_ShouldSwallowException()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Database unavailable."));

        // Act
        var act = () => _notificationCreator.CreateAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            NotificationType.AddedToTeam,
            Guid.NewGuid(),
            "You were added to the team \"Platform\".",
            TestContext.Current.CancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CreateAsync_WhenCategoryIsMuted_ShouldNotCreateNotification()
    {
        // Arrange
        var recipientId = Guid.NewGuid();
        var mutedPreference = new NotificationPreference(recipientId);
        mutedPreference.Update(
            mentionsEnabled: true,
            assignedWorkItemChangesEnabled: true,
            workItemCommentsEnabled: true,
            membershipEnabled: false);
        _notificationPreferenceRepository
            .GetByUserIdAsync(recipientId, Arg.Any<CancellationToken>())
            .Returns(mutedPreference);

        // Act
        await _notificationCreator.CreateAsync(
            recipientId,
            Guid.NewGuid(),
            NotificationType.AddedToProject,
            Guid.NewGuid(),
            "You were added to the project \"Mirai\".",
            TestContext.Current.CancellationToken);

        // Assert
        await _notificationRepository.DidNotReceive().AddAsync(
            Arg.Any<Notification>(),
            Arg.Any<CancellationToken>());
    }
}
