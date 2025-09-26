using Domain.Shared;

namespace Domain.Teams;

public interface ITeamRepository : IRepository<Team>
{
    Task<Team?> GetByIdWithRetrospectivesAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}