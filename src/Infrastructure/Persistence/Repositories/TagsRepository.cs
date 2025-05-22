using Application.Common.Interfaces.Persistence;
using Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class TagsRepository : Repository<Tag>, ITagsRepository
{
    public TagsRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Tag?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<List<Tag>> ListByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tags
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }
}