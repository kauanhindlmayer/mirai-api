using Domain.Sprints;

namespace Infrastructure.Persistence.Repositories;

internal sealed class SprintsRepository : Repository<Sprint>, ISprintsRepository
{
    public SprintsRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}