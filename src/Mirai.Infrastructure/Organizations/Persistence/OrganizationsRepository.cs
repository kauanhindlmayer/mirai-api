using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Organizations;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.Organizations.Persistence;

public class OrganizationsRepository(AppDbContext dbContext) : IOrganizationsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(Organization organization, CancellationToken cancellationToken)
    {
        await _dbContext.AddAsync(organization, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        return await _dbContext.Organizations
            .Include(o => o.Projects)
            .FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken);
    }

    public Task<List<Organization>> ListAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Organizations.ToListAsync(cancellationToken);
    }

    public Task RemoveAsync(Organization organization, CancellationToken cancellationToken)
    {
        _dbContext.Remove(organization);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(Organization organization, CancellationToken cancellationToken)
    {
        _dbContext.Update(organization);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
