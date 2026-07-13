using Application.Abstractions.Authentication;
using Domain.Notifications;
using ErrorOr;
using MediatR;

namespace Application.Notifications.Queries.GetNotificationPreferences;

internal sealed class GetNotificationPreferencesQueryHandler
    : IRequestHandler<GetNotificationPreferencesQuery, ErrorOr<NotificationPreferenceResponse>>
{
    private readonly INotificationPreferenceRepository _notificationPreferenceRepository;
    private readonly IUserContext _userContext;

    public GetNotificationPreferencesQueryHandler(
        INotificationPreferenceRepository notificationPreferenceRepository,
        IUserContext userContext)
    {
        _notificationPreferenceRepository = notificationPreferenceRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<NotificationPreferenceResponse>> Handle(
        GetNotificationPreferencesQuery query,
        CancellationToken cancellationToken)
    {
        var preference = await _notificationPreferenceRepository.GetByUserIdAsync(
            _userContext.UserId,
            cancellationToken);

        return new NotificationPreferenceResponse
        {
            MentionsEnabled = preference?.MentionsEnabled ?? true,
            AssignedWorkItemChangesEnabled = preference?.AssignedWorkItemChangesEnabled ?? true,
            WorkItemCommentsEnabled = preference?.WorkItemCommentsEnabled ?? true,
            MembershipEnabled = preference?.MembershipEnabled ?? true,
        };
    }
}
