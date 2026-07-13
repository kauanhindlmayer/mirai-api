namespace Presentation.Controllers.Notifications;

/// <summary>
/// Query parameters for filtering notifications.
/// </summary>
public class NotificationsQueryParameters : PageRequest
{
    /// <summary>
    /// Whether to return only unread notifications.
    /// </summary>
    public bool UnreadOnly { get; init; }
}
