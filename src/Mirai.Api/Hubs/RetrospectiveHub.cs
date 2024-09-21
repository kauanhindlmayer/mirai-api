using Microsoft.AspNetCore.SignalR;
using Mirai.Contracts.Retrospectives;

namespace Mirai.Api.Hubs;

public class RetrospectiveHub : Hub<IRetrospectiveHub>
{
    private static int _activeClientsCount;

    public async Task SendRetrospectiveItem(RetrospectiveItemResponse retrospectiveItem)
    {
        await Clients.All.SendRetrospectiveItem(retrospectiveItem);
    }

    public override async Task OnConnectedAsync()
    {
        _activeClientsCount++;
        await Clients.All.UpdateClientsCount(_activeClientsCount);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _activeClientsCount--;
        await Clients.All.UpdateClientsCount(_activeClientsCount);
        await base.OnDisconnectedAsync(exception);
    }
}