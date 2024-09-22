using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WorkItems;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.Tags.Persistence;

public class TagsRepository(AppDbContext dbContext) : ITagsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }
}