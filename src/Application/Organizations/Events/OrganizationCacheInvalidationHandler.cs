using Application.Abstractions.Caching;
using Domain.Organizations;
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
        return InvalidateCache(notification.Organization, cancellationToken);
    }

    public Task Handle(
        OrganizationUpdatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Organization, cancellationToken);
    }

    public Task Handle(
        OrganizationDeletedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        return InvalidateCache(notification.Organization, cancellationToken);
    }

    private async Task InvalidateCache(
        Organization organization,
        CancellationToken cancellationToken)
    {
        var organizationKey = CacheKeys.GetOrganizationKey(organization.Id);
        await _cacheService.RemoveAsync(organizationKey, cancellationToken);
        await _cacheService.RemoveByPatternAsync("organizations:*", cancellationToken);
    }
}
