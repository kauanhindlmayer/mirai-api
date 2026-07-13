using Application.Abstractions.Authentication;
using Domain.Notifications;
using Domain.Organizations.Events;
using MediatR;

namespace Application.Notifications.Events;

internal sealed class UserAddedToOrganizationNotificationHandler
    : INotificationHandler<UserAddedToOrganizationDomainEvent>
{
    private readonly NotificationCreator _notificationCreator;
    private readonly IUserContext _userContext;

    public UserAddedToOrganizationNotificationHandler(
        NotificationCreator notificationCreator,
        IUserContext userContext)
    {
        _notificationCreator = notificationCreator;
        _userContext = userContext;
    }

    public Task Handle(
        UserAddedToOrganizationDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return _notificationCreator.CreateAsync(
            notification.User.Id,
            _userContext.UserId,
            NotificationType.AddedToOrganization,
            notification.Organization.Id,
            $"You were added to the organization \"{notification.Organization.Name}\".",
            cancellationToken);
    }
}
