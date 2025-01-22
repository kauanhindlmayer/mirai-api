using Domain.Projects;

namespace Application.Common.Interfaces.Persistence;

public interface IProjectsRepository : IRepository<Project>
{
    Task<Project?> GetByIdWithWorkItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdWithWikiPagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdWithTagsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Project>> ListAsync(Guid organizationId, CancellationToken cancellationToken = default);
}