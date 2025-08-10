using Domain.Shared;

namespace Domain.Organizations;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> GetByIdWithProjectsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Organization?> GetByIdWithProjectsAndUsersAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}