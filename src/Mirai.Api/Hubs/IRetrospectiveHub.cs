using Mirai.Contracts.Retrospectives;
using SignalRSwaggerGen.Attributes;

namespace Mirai.Api.Hubs;

[SignalRHub(path: "/hubs/retrospective", tag: "TeamRetrospective")]
public interface IRetrospectiveHub
{
    /// <summary>
    /// Sends a retrospective item to all connected clients.
    /// </summary>
    /// <param name="retrospectiveItem">The retrospective item to send.</param>
    [SignalRMethod(name: "send-retrospective-item")]
    Task SendRetrospectiveItem(RetrospectiveItemResponse retrospectiveItem);

    /// <summary>
    /// Updates the connected clients count for all connected clients.
    /// </summary>
    /// <param name="count">The number of connected clients.</param>
    [SignalRMethod(name: "update-connected-clients-count")]
    Task UpdateConnectedClientsCount(int count);
}