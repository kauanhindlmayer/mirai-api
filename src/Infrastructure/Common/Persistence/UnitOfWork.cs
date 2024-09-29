using Application.Common.Interfaces;

namespace Infrastructure.Common.Persistence;

public sealed class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}