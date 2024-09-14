using Mirai.Domain.Organizations;

namespace Mirai.Application.Common.Interfaces;

public interface IOrganizationsRepository
{
    Task AddAsync(Organization organization, CancellationToken cancellationToken);
    Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken);
    Task<List<Organization>> ListAsync(CancellationToken cancellationToken);
    Task RemoveAsync(Organization organization, CancellationToken cancellationToken);
    Task UpdateAsync(Organization organization, CancellationToken cancellationToken);
}