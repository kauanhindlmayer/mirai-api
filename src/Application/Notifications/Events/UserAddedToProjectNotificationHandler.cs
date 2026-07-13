using Application.Abstractions.Authentication;
using Domain.Notifications;
using Domain.Projects.Events;
using MediatR;

namespace Application.Notifications.Events;

internal sealed class UserAddedToProjectNotificationHandler
    : INotificationHandler<UserAddedToProjectDomainEvent>
{
    private readonly NotificationCreator _notificationCreator;
    private readonly IUserContext _userContext;

    public UserAddedToProjectNotificationHandler(
        NotificationCreator notificationCreator,
        IUserContext userContext)
    {
        _notificationCreator = notificationCreator;
        _userContext = userContext;
    }

    public Task Handle(
        UserAddedToProjectDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return _notificationCreator.CreateAsync(
            notification.User.Id,
            _userContext.UserId,
            NotificationType.AddedToProject,
            notification.Project.Id,
            $"You were added to the project \"{notification.Project.Name}\".",
            cancellationToken);
    }
}
