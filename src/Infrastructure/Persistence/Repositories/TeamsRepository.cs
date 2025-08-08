using Domain.Teams;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class TeamsRepository : Repository<Team>, ITeamsRepository
{
    public TeamsRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public new async Task<Team?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Team?> GetByIdWithRetrospectivesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teams
            .Include(t => t.Retrospectives)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
}
