using System.Linq.Expressions;
using Application.Common;
using Application.Common.Interfaces.Persistence;
using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class WorkItemsRepository : Repository<WorkItem>, IWorkItemsRepository
{
    public WorkItemsRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public new async Task<WorkItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkItems
            .Include(wi => wi.Comments)
            .Include(wi => wi.Tags)
            .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);
    }

    public async Task<int> GetNextWorkItemCodeAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var workItemCount = await _dbContext.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == projectId)
            .CountAsync(cancellationToken);

        return workItemCount + 1;
    }

    public Task<PaginatedList<WorkItem>> PaginatedListAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        string? sortField,
        string? sortOrder,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == projectId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(wi => wi.Title.ToLower().Contains(searchTerm.ToLower()));
        }

        if (sortOrder?.ToLower() == "desc")
        {
            query = query.OrderByDescending(GetSortProperty(sortField));
        }
        else
        {
            query = query.OrderBy(GetSortProperty(sortField));
        }

        return PaginatedList<WorkItem>.CreateAsync(
            query,
            pageNumber,
            pageSize,
            cancellationToken);
    }

    public Task<List<WorkItem>> SearchAsync(
        Guid projectId,
        float[] searchTermEmbedding,
        int topK = 10,
        CancellationToken cancellationToken = default)
    {
        var searchTermVector = new Vector(searchTermEmbedding);

        return _dbContext.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == projectId && wi.SearchVector != null)
            .OrderBy(wi => wi.SearchVector!.CosineDistance(searchTermVector))
            .Take(topK)
            .ToListAsync(cancellationToken);
    }

    private static Expression<Func<WorkItem, object>> GetSortProperty(string? sortField)
    {
        return sortField?.ToLower() switch
        {
            "title" => wi => wi.Title,
            "status" => wi => wi.Status,
            "type" => wi => wi.Type,
            "activityDate" => wi => wi.UpdatedAt ?? wi.CreatedAt,
            _ => wi => wi.Code,
        };
    }
}
