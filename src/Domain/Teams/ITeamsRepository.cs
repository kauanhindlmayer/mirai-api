using Domain.Shared;

namespace Domain.Teams;

public interface ITeamsRepository : IRepository<Team>
{
    Task<Team?> GetByIdWithRetrospectivesAsync(Guid id, CancellationToken cancellationToken = default);
}