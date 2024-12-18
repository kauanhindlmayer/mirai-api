using Domain.Teams;

namespace Application.Common.Interfaces.Persistence;

public interface ITeamsRepository : IRepository<Team>
{
    Task<Team?> GetByIdWithRetrospectivesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Team>> ListAsync(Guid projectId, CancellationToken cancellationToken = default);
}