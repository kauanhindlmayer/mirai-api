using Microsoft.AspNetCore.SignalR;
using Mirai.Contracts.Retrospectives;

namespace Mirai.Api.Hubs;

public class RetrospectiveHub : Hub<IRetrospectiveHub>
{
    private static int _connectedClientsCount;

    public async Task SendRetrospectiveItem(RetrospectiveItemResponse retrospectiveItem)
    {
        await Clients.All.SendRetrospectiveItem(retrospectiveItem);
    }

    public async Task DeleteRetrospectiveItem(Guid retrospectiveItemId)
    {
        await Clients.All.DeleteRetrospectiveItem(retrospectiveItemId);
    }

    public override async Task OnConnectedAsync()
    {
        _connectedClientsCount++;
        await Clients.All.UpdateConnectedClientsCount(_connectedClientsCount);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connectedClientsCount--;
        await Clients.All.UpdateConnectedClientsCount(_connectedClientsCount);
        await base.OnDisconnectedAsync(exception);
    }
}