using Application.Common;
using Application.Common.Interfaces.Services;
using Domain.Organizations.Events;
using MediatR;

namespace Application.Organizations.Events;

internal sealed class OrganizationCacheInvalidationHandler :
    INotificationHandler<OrganizationCreatedDomainEvent>,
    INotificationHandler<OrganizationUpdatedDomainEvent>,
    INotificationHandler<OrganizationDeletedDomainEvent>
{
    private readonly ICacheService _cacheService;

    public OrganizationCacheInvalidationHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public Task Handle(
        OrganizationCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Organization.Id, cancellationToken);
    }

    public Task Handle(
        OrganizationUpdatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Organization.Id, cancellationToken);
    }

    public Task Handle(
        OrganizationDeletedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Organization.Id, cancellationToken);
    }

    private async Task InvalidateCache(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var organizationKey = CacheKeys.GetOrganizationKey(organizationId);
        await _cacheService.RemoveAsync(organizationKey, cancellationToken);

        var organizationsKey = CacheKeys.GetOrganizationsKey();
        await _cacheService.RemoveAsync(organizationsKey, cancellationToken);
    }
}
