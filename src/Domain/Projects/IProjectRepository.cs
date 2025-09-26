using Domain.Shared;

namespace Domain.Projects;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByIdWithOrganizationAndUsersAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdWithUsersAndTeamsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdWithWorkItemsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdWithWikiPagesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdWithTagsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdWithTeamsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdWithPersonasAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}