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

    public Task<Project?> GetByIdWithTeamsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Projects
            .Include(p => p.Teams)
            .ThenInclude(t => t.Board)
            .ThenInclude(b => b.Columns)
            .ThenInclude(c => c.Cards)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetByIdWithPersonasAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .Include(p => p.Personas)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
