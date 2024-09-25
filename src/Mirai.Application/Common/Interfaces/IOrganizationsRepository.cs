using Mirai.Domain.Organizations;

namespace Mirai.Application.Common.Interfaces;

public interface IOrganizationsRepository
{
    Task AddAsync(Organization organization, CancellationToken cancellationToken = default);
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Organization>> ListAsync(CancellationToken cancellationToken = default);
    void Remove(Organization organization);
    void Update(Organization organization);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}