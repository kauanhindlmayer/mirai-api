using Domain.Organizations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class OrganizationRepository : Repository<Organization>,
    IOrganizationRepository
{
    public OrganizationRepository(ApplicationDbContext dbContext)
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
            .Include(o => o.Members)
                .ThenInclude(m => m.User)
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

    public Task<bool> IsUserInOrganizationAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Organizations
            .AsNoTracking()
            .Where(o => o.Id == organizationId)
            .AnyAsync(o => o.Members.Any(m => m.UserId == userId), cancellationToken);
    }
}
