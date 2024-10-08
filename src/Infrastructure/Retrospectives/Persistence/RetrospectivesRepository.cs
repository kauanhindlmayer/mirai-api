using Application.Common.Interfaces;
using Domain.Retrospectives;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Retrospectives.Persistence;

public class RetrospectivesRepository(AppDbContext dbContext) : IRetrospectivesRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(Retrospective retrospective, CancellationToken cancellationToken = default)
    {
        await _dbContext.Retrospectives.AddAsync(retrospective, cancellationToken);
    }

    public Task<Retrospective?> GetByIdWithColumnsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Retrospectives
            .Include(x => x.Columns)
                .ThenInclude(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Retrospective>> ListAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Retrospectives
            .AsNoTracking()
            .Where(x => x.TeamId == teamId)
            .ToListAsync(cancellationToken);
    }

    public void Update(Retrospective retrospective)
    {
        _dbContext.Retrospectives.Update(retrospective);
    }

    public void Remove(Retrospective retrospective)
    {
        _dbContext.Retrospectives.Remove(retrospective);
    }
}