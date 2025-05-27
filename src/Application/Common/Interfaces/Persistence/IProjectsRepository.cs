using Domain.Projects;

namespace Application.Common.Interfaces.Persistence;

public interface IProjectsRepository : IRepository<Project>
{
    Task<Project?> GetByIdWithWorkItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdWithWikiPagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdWithTagsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdWithTeamsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdWithPersonasAsync(Guid id, CancellationToken cancellationToken = default);
}