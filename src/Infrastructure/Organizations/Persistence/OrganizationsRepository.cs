using Application.Common.Interfaces;
using Domain.Organizations;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Organizations.Persistence;

public class OrganizationsRepository(AppDbContext dbContext) : IOrganizationsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(organization, cancellationToken);
    }

    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Organization?> GetByIdWithProjectsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .Include(o => o.Projects)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public Task<List<Organization>> ListAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Organizations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public void Remove(Organization organization)
    {
        _dbContext.Remove(organization);
    }

    public void Update(Organization organization)
    {
        _dbContext.Update(organization);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(o => o.Name == name, cancellationToken);
    }
}
