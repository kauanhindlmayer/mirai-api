using Domain.Retrospectives;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class RetrospectiveHub : Hub<IRetrospectiveHub>
{
    private int _connectedClientsCount;

    public async Task SendRetrospectiveItem(RetrospectiveItem retrospectiveItem)
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