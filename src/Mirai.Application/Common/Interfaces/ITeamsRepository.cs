using Mirai.Domain.Teams;

namespace Mirai.Application.Common.Interfaces;

public interface ITeamsRepository
{
    Task AddAsync(Team team, CancellationToken cancellationToken);
    Task<Team> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Team>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken);
    Task UpdateAsync(Team team, CancellationToken cancellationToken);
    Task DeleteAsync(Team team, CancellationToken cancellationToken);
}