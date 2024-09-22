using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Teams;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.Teams.Persistence;

public class TeamsRepository(AppDbContext dbContext) : ITeamsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(Team team, CancellationToken cancellationToken)
    {
        await _dbContext.Teams.AddAsync(team, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(Team team, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Team> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<Team>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Team team, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
