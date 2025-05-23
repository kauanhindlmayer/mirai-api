using Domain.Organizations;

namespace Application.Common.Interfaces.Persistence;

public interface IOrganizationsRepository : IRepository<Organization>
{
    Task<Organization?> GetByIdWithProjectsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}