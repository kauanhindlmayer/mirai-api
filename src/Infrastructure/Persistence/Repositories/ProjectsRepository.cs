using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class ProjectsRepository : Repository<Project>, IProjectsRepository
{
    public ProjectsRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Project?> GetByIdWithBoardsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .Include(p => p.Boards)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetByIdWithWorkItemsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .Include(p => p.WorkItems)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetByIdWithWikiPagesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .Include(p => p.WikiPages)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetByIdWithTagsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .Include(p => p.Tags)
                .ThenInclude(t => t.WorkItems)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<List<Project>> ListAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }
}
