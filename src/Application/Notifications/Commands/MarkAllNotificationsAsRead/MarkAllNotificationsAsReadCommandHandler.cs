using Application.Abstractions.Authentication;
using Domain.Notifications;
using ErrorOr;
using MediatR;

namespace Application.Notifications.Commands.MarkAllNotificationsAsRead;

internal sealed class MarkAllNotificationsAsReadCommandHandler
    : IRequestHandler<MarkAllNotificationsAsReadCommand, ErrorOr<Success>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserContext _userContext;

    public MarkAllNotificationsAsReadCommandHandler(
        INotificationRepository notificationRepository,
        IUserContext userContext)
    {
        _notificationRepository = notificationRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Success>> Handle(
        MarkAllNotificationsAsReadCommand command,
        CancellationToken cancellationToken)
    {
        var unreadNotifications = await _notificationRepository.GetUnreadByRecipientIdAsync(
            _userContext.UserId,
            cancellationToken);

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
            _notificationRepository.Update(notification);
        }

        return Result.Success;
    }
}
