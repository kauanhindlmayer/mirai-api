using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public abstract class Repository<T>(ApplicationDbContext dbContext)
    where T : Entity
{
    protected readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Set<T>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext
            .Set<T>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
    }

    public void Remove(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }
}