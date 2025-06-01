using Domain.Retrospectives;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Presentation.Hubs;

[SignalRHub(path: "/hubs/retrospective", tag: "TeamRetrospective")]
public interface IRetrospectiveHub
{
    /// <summary>
    /// Sends a retrospective item to all clients.
    /// </summary>
    /// <param name="retrospectiveItem">The retrospective item to send.</param>
    [SignalRMethod(name: "send-retrospective-item")]
    [HubMethodName("send-retrospective-item")]
    Task SendRetrospectiveItem(RetrospectiveItem retrospectiveItem);

    /// <summary>
    /// Notifies clients to delete a retrospective item.
    /// </summary>
    /// <param name="retrospectiveItemId">The unique identifier of the retrospective item.</param>
    [SignalRMethod(name: "delete-retrospective-item")]
    [HubMethodName("delete-retrospective-item")]
    Task DeleteRetrospectiveItem(Guid retrospectiveItemId);

    /// <summary>
    /// Notifies clients of the current connection count.
    /// </summary>
    /// <param name="count">The number of connected clients.</param>
    [SignalRMethod(name: "update-connected-clients-count")]
    [HubMethodName("update-connected-clients-count")]
    Task UpdateConnectedClientsCount(int count);
}