using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.Projects.Persistence;

public class ProjectsRepository(AppDbContext dbContext) : IProjectsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(Project project, CancellationToken cancellationToken)
    {
        await _dbContext.AddAsync(project, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // TODO: Refactor to use a specification pattern or inject the dbContext and
    // load the related entities in the application layer. This method should not
    // be responsible for loading all the related entities.
    public async Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .Include(p => p.WorkItems)
            .Include(p => p.WikiPages)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
    }

    public Task<List<Project>> ListAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        return _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken)
    {
        _dbContext.Update(project);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Project project, CancellationToken cancellationToken)
    {
        _dbContext.Remove(project);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
