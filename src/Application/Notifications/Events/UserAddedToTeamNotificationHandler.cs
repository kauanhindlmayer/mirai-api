using Application.Abstractions.Authentication;
using Domain.Notifications;
using Domain.Teams.Events;
using MediatR;

namespace Application.Notifications.Events;

internal sealed class UserAddedToTeamNotificationHandler
    : INotificationHandler<UserAddedToTeamDomainEvent>
{
    private readonly NotificationCreator _notificationCreator;
    private readonly IUserContext _userContext;

    public UserAddedToTeamNotificationHandler(
        NotificationCreator notificationCreator,
        IUserContext userContext)
    {
        _notificationCreator = notificationCreator;
        _userContext = userContext;
    }

    public Task Handle(
        UserAddedToTeamDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return _notificationCreator.CreateAsync(
            notification.User.Id,
            _userContext.UserId,
            NotificationType.AddedToTeam,
            notification.Team.Id,
            $"You were added to the team \"{notification.Team.Name}\".",
            cancellationToken);
    }
}
