using Application.Common.Interfaces;
using Domain.Projects;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Projects.Persistence;

public class ProjectsRepository(AppDbContext dbContext) : IProjectsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(project, cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetByIdWithBoardsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .Include(p => p.Boards)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetByIdWithWorkItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .Include(p => p.WorkItems)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetByIdWithWikiPagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .Include(p => p.WikiPages)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetByIdWithTagsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<List<Project>> ListAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }

    public void Update(Project project)
    {
        _dbContext.Update(project);
    }

    public void Remove(Project project)
    {
        _dbContext.Remove(project);
    }
}
