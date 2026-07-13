using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

/// <summary>
/// Pure server-push hub: notifications are pushed via <see cref="NotificationRealtimeNotifier"/>
/// to a specific recipient (<c>Clients.User(...)</c>), never broadcast - clients only ever
/// subscribe, they don't invoke methods on this hub.
/// </summary>
[Authorize]
public sealed class NotificationHub : Hub<INotificationHub>
{
}
