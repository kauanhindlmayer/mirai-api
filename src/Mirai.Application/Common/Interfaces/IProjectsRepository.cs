using Mirai.Domain.Projects;

namespace Mirai.Application.Common.Interfaces;

public interface IProjectsRepository
{
    Task<Project> AddAsync(Project project, CancellationToken cancellationToken);
    Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken);
    Task<List<Project>> ListAsync(CancellationToken cancellationToken);
    Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken);
    Task DeleteAsync(Project project, CancellationToken cancellationToken);
}