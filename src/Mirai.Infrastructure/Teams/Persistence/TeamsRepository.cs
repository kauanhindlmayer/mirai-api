using Microsoft.EntityFrameworkCore;
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

    public async Task<Team?> GetByIdAsync(Guid teamId, CancellationToken cancellationToken)
    {
        return await _dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);
    }

    public async Task<List<Team>> ListAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await _dbContext.Teams
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Team team, CancellationToken cancellationToken)
    {
        _dbContext.Teams.Update(team);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Team team, CancellationToken cancellationToken)
    {
        _dbContext.Teams.Remove(team);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
