using Mirai.Domain.Projects;

namespace Mirai.Application.Common.Interfaces;

public interface IProjectsRepository
{
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdWithWorkItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdWithWikiPagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdWithTagsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Project>> ListAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
    Task RemoveAsync(Project project, CancellationToken cancellationToken = default);
}