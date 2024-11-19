using Application.Common.Caching;
using Application.Common.Interfaces;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.DeleteOrganization;

public class DeleteOrganizationCommandHandler(
    IOrganizationsRepository _organizationsRepository,
    ICacheService _cacheService)
    : IRequestHandler<DeleteOrganizationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        _organizationsRepository.Remove(organization);

        InvalidateCache(organization.Id);

        return Result.Success;
    }

    // TODO: Refactor to avoid code duplication
    private void InvalidateCache(Guid organizationId)
    {
        var organizationsCacheKey = CacheKeys.GetOrganizationKey(organizationId);
        _cacheService.RemoveAsync(organizationsCacheKey, CancellationToken.None);

        var organizationCacheKey = CacheKeys.GetOrganizationKey(organizationId);
        _cacheService.RemoveAsync(organizationCacheKey, CancellationToken.None);
    }
}