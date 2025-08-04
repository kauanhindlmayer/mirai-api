using Application.Common.Interfaces.Persistence;
using Domain.Organizations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class OrganizationsRepository : Repository<Organization>,
    IOrganizationsRepository
{
    public OrganizationsRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Organization?> GetByIdWithProjectsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .Include(o => o.Projects)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Organization?> GetByIdWithProjectsAndUsersAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .Include(o => o.Projects)
            .Include(o => o.Users)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(o => o.Name == name, cancellationToken);
    }
}
