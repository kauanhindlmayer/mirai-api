using Mirai.Domain.Teams;

namespace Mirai.Application.Common.Interfaces;

public interface ITeamsRepository
{
    Task AddAsync(Team team, CancellationToken cancellationToken = default);
    Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Team>> ListAsync(Guid projectId, CancellationToken cancellationToken = default);
    void Update(Team team);
    void Remove(Team team);
}