using Domain.Sprints;

namespace Infrastructure.Persistence.Repositories;

internal sealed class SprintRepository : Repository<Sprint>, ISprintRepository
{
    public SprintRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}