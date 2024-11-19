using Application.Common.Interfaces;
using Domain.Organizations;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Organizations.Persistence;

public class OrganizationsRepository : Repository<Organization>, IOrganizationsRepository
{
    public OrganizationsRepository(AppDbContext dbContext)
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

    public Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(o => o.Name == name, cancellationToken);
    }
}
