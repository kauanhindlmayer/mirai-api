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

    public async Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .Include(p => p.WorkItems)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
    }

    public Task<List<Project>> ListAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Projects.ToListAsync(cancellationToken);
    }

    public Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken)
    {
        _dbContext.Update(project);
        return Task.FromResult(project);
    }

    public Task RemoveAsync(Project project, CancellationToken cancellationToken)
    {
        _dbContext.Remove(project);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
