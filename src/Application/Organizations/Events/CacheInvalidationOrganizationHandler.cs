using Application.Common;
using Application.Common.Interfaces.Services;
using Domain.Organizations.Events;
using MediatR;

namespace Application.Organizations.Events;

internal sealed class CacheInvalidationOrganizationHandler(ICacheService cacheService)
    : INotificationHandler<OrganizationCreatedDomainEvent>,
    INotificationHandler<OrganizationUpdatedDomainEvent>,
    INotificationHandler<OrganizationDeletedDomainEvent>
{
    private readonly ICacheService _cacheService = cacheService;

    public Task Handle(OrganizationCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Id, cancellationToken);
    }

    public Task Handle(OrganizationUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Id, cancellationToken);
    }

    public Task Handle(OrganizationDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Id, cancellationToken);
    }

    private async Task InvalidateCache(Guid organizationId, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveAsync(CacheKeys.GetOrganizationKey(organizationId), cancellationToken);
        await _cacheService.RemoveAsync(CacheKeys.GetOrganizationsKey(), cancellationToken);
    }
}
