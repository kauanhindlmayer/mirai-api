using Application.Abstractions.Authentication;
using Domain.Notifications;
using ErrorOr;
using MediatR;

namespace Application.Notifications.Commands.MarkNotificationAsRead;

internal sealed class MarkNotificationAsReadCommandHandler
    : IRequestHandler<MarkNotificationAsReadCommand, ErrorOr<Success>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserContext _userContext;

    public MarkNotificationAsReadCommandHandler(
        INotificationRepository notificationRepository,
        IUserContext userContext)
    {
        _notificationRepository = notificationRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Success>> Handle(
        MarkNotificationAsReadCommand command,
        CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.GetByIdAsync(
            command.NotificationId,
            cancellationToken);

        if (notification is null || notification.RecipientUserId != _userContext.UserId)
        {
            return NotificationErrors.NotFound;
        }

        notification.MarkAsRead();
        _notificationRepository.Update(notification);

        return Result.Success;
    }
}
