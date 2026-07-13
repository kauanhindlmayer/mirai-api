using ErrorOr;
using MediatR;

namespace Application.Notifications.Commands.UpdateNotificationPreferences;

public sealed record UpdateNotificationPreferencesCommand(
    bool MentionsEnabled,
    bool AssignedWorkItemChangesEnabled,
    bool WorkItemCommentsEnabled,
    bool MembershipEnabled) : IRequest<ErrorOr<Success>>;
