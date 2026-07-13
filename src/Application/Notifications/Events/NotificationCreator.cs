using Domain.Notifications;
using Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Notifications.Events;

internal sealed class NotificationCreator
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationCreator> _logger;

    public NotificationCreator(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        ILogger<NotificationCreator> logger)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task CreateAsync(
        Guid recipientUserId,
        Guid actorUserId,
        NotificationType type,
        Guid entityId,
        string message,
        CancellationToken cancellationToken)
    {
        if (recipientUserId == actorUserId)
        {
            return;
        }

        try
        {
            var notification = new Notification(recipientUserId, type, entityId, message);
            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while creating a {NotificationType} notification for user {RecipientUserId}.",
                type,
                recipientUserId);
        }
    }
}
