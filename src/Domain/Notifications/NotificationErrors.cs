using ErrorOr;

namespace Domain.Notifications;

public static class NotificationErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "Notification.NotFound",
        description: "Notification not found.");
}
