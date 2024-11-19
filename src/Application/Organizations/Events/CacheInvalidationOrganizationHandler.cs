using Application.Common;
using Application.Common.Interfaces.Services;
using Domain.Organizations.Events;
using MediatR;

namespace Application.Organizations.Events;

internal class CacheInvalidationOrganizationHandler(ICacheService cacheService)
    : INotificationHandler<OrganizationCreatedEvent>,
    INotificationHandler<OrganizationUpdatedEvent>,
    INotificationHandler<OrganizationDeletedEvent>
{
    private readonly ICacheService _cacheService = cacheService;

    public Task Handle(OrganizationCreatedEvent notification, CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Id, cancellationToken);
    }

    public Task Handle(OrganizationUpdatedEvent notification, CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Id, cancellationToken);
    }

    public Task Handle(OrganizationDeletedEvent notification, CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Id, cancellationToken);
    }

    private async Task InvalidateCache(Guid organizationId, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveAsync(CacheKeys.GetOrganizationKey(organizationId), cancellationToken);
        await _cacheService.RemoveAsync(CacheKeys.GetOrganizationsKey(), cancellationToken);
    }
}
