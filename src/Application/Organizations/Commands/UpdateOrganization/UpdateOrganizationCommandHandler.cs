using Application.Common.Caching;
using Application.Common.Interfaces;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Commands.UpdateOrganization;

public class UpdateOrganizationCommandHandler(
    IOrganizationsRepository _organizationsRepository,
    ICacheService _cacheService)
    : IRequestHandler<UpdateOrganizationCommand, ErrorOr<Organization>>
{
    public async Task<ErrorOr<Organization>> Handle(
        UpdateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        organization.Update(command.Name, command.Description);
        _organizationsRepository.Update(organization);

        InvalidateCache(organization.Id);

        return organization;
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