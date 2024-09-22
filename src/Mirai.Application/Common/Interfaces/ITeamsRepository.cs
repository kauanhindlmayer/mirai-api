using Mirai.Domain.Teams;

namespace Mirai.Application.Common.Interfaces;

public interface ITeamsRepository
{
    Task AddAsync(Team team, CancellationToken cancellationToken);
    Task<Team?> GetByIdAsync(Guid teamId, CancellationToken cancellationToken);
    Task<List<Team>> ListAsync(Guid projectId, CancellationToken cancellationToken);
    Task UpdateAsync(Team team, CancellationToken cancellationToken);
    Task DeleteAsync(Team team, CancellationToken cancellationToken);
}