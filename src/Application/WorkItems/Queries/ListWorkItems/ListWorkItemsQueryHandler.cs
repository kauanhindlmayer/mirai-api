using Application.Abstractions;
using Application.Abstractions.Mappings;
using Application.Abstractions.Sorting;
using Domain.Shared;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.ListWorkItems;

internal sealed class ListWorkItemsQueryHandler
    : IRequestHandler<ListWorkItemsQuery, ErrorOr<PaginatedList<WorkItemBriefResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly SortMappingProvider _sortMappingProvider;

    public ListWorkItemsQueryHandler(
        IApplicationDbContext context,
        SortMappingProvider sortMappingProvider)
    {
        _context = context;
        _sortMappingProvider = sortMappingProvider;
    }

    public async Task<ErrorOr<PaginatedList<WorkItemBriefResponse>>> Handle(
        ListWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        if (!_sortMappingProvider.ValidateMappings<WorkItemBriefResponse, WorkItem>(query.Sort))
        {
            return Errors.InvalidSort(query.Sort);
        }

        var workItemsQuery = _context.WorkItems.Where(wi => wi.ProjectId == query.ProjectId);
        query.SearchTerm ??= query.SearchTerm?.Trim().ToLower();

        var sortMappings = _sortMappingProvider.GetMappings<WorkItemBriefResponse, WorkItem>();

        var workItems = await workItemsQuery
            .Where(wi => query.SearchTerm == null || wi.Title.ToLower().Contains(query.SearchTerm))
            .Where(wi => query.Type == null || wi.Type == query.Type)
            .Where(wi => query.Status == null || wi.Status == query.Status)
            .Where(wi => query.AssigneeId == null || wi.AssignedUserId == query.AssigneeId)
            .ApplySorting(query.Sort, sortMappings)
            .Select(WorkItemQueries.ProjectToBriefDto())
            .PaginatedListAsync(query.Page, query.PageSize, cancellationToken);

        return workItems;
    }
}