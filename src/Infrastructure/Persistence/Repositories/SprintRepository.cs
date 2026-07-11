using Domain.Sprints;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class SprintRepository : Repository<Sprint>, ISprintRepository
{
    public SprintRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Sprint?> GetByIdWithWorkItemsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sprints
            .Include(s => s.WorkItems)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}