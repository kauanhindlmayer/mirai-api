using Application.Common.Interfaces;
using Domain.Teams;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Teams.Persistence;

public class TeamsRepository(AppDbContext dbContext) : ITeamsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        await _dbContext.Teams.AddAsync(team, cancellationToken);
    }

    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Team>> ListAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Teams
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public void Update(Team team)
    {
        _dbContext.Teams.Update(team);
    }

    public void Remove(Team team)
    {
        _dbContext.Teams.Remove(team);
    }
}
