using Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Project?> GetByIdWithOrganizationAndUsersAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Projects
            .Include(p => p.Organization)
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Project?> GetByIdWithUsersAndTeamsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Projects
            .Include(p => p.Users)
            .Include(p => p.Teams)
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
