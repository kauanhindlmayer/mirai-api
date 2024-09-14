using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.Projects.Persistence;

public class ProjectsRepository(AppDbContext dbContext) : IProjectsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<Project> AddAsync(Project project, CancellationToken cancellationToken)
    {
        _dbContext.Projects.Add(project);
        return Task.FromResult(project);
    }

    public Task DeleteAsync(Project project, CancellationToken cancellationToken)
    {
        _dbContext.Remove(project);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await _dbContext.Projects.FindAsync(projectId, cancellationToken);
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
}
