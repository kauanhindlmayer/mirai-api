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

    public Task<List<Tag>> GetByProjectAsync(
        Guid projectId,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Tags
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t => t.Name.Contains(searchTerm));
        }

        return query.ToListAsync(cancellationToken);
    }
}