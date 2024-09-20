using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Retrospectives;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.Retrospectives.Persistence;

public class RetrospectivesRepository(AppDbContext dbContext) : IRetrospectivesRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(Retrospective retrospective, CancellationToken cancellationToken)
    {
        await _dbContext.Retrospectives.AddAsync(retrospective, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Retrospective?> GetByIdAsync(Guid retrospectiveId, CancellationToken cancellationToken)
    {
        return _dbContext.Retrospectives
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == retrospectiveId, cancellationToken);
    }
}