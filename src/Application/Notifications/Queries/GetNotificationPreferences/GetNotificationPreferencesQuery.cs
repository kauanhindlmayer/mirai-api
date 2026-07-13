using ErrorOr;
using MediatR;

namespace Application.Notifications.Queries.GetNotificationPreferences;

public sealed record GetNotificationPreferencesQuery : IRequest<ErrorOr<NotificationPreferenceResponse>>;
