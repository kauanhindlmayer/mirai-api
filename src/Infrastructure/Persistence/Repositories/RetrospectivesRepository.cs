using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class RetrospectivesRepository : Repository<Retrospective>,
    IRetrospectivesRepository
{
    public RetrospectivesRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Retrospective?> GetByIdWithColumnsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Retrospectives
            .Include(x => x.Columns)
                .ThenInclude(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Retrospective>> ListAsync(
        Guid teamId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Retrospectives
            .AsNoTracking()
            .Where(x => x.TeamId == teamId)
            .ToListAsync(cancellationToken);
    }
}