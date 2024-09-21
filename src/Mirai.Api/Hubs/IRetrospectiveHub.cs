using Mirai.Contracts.Retrospectives;

namespace Mirai.Api.Hubs;

public interface IRetrospectiveHub
{
    Task SendRetrospectiveItem(RetrospectiveItemResponse retrospectiveItem);
    Task UpdateClientsCount(int count);
}