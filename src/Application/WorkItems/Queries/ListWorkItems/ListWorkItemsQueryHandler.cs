using System.Linq.Expressions;
using Application.Common;
using Application.Common.Interfaces.Persistence;
using Application.Common.Mappings;
using Application.WorkItems.Queries.SearchWorkItems;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.ListWorkItems;

internal sealed class ListWorkItemsQueryHandler
    : IRequestHandler<ListWorkItemsQuery, ErrorOr<PaginatedList<WorkItemBriefResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListWorkItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<PaginatedList<WorkItemBriefResponse>>> Handle(
        ListWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var workItemsQuery = _context.WorkItems.Where(wi => wi.ProjectId == query.ProjectId);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            workItemsQuery = workItemsQuery.Where(wi => wi.Title.ToLower().Contains(query.SearchTerm.ToLower()));
        }

        var isDescending = query.SortOrder?.ToLower() == "desc";
        var sortProperty = GetSortProperty(query.SortField);
        workItemsQuery = isDescending
            ? workItemsQuery.OrderByDescending(sortProperty)
            : workItemsQuery.OrderBy(sortProperty);

        var workItems = await workItemsQuery
            .Select(wi => ToDto(wi))
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        return workItems;
    }

    private static WorkItemBriefResponse ToDto(WorkItem workItem)
    {
        return new WorkItemBriefResponse
        {
            Id = workItem.Id,
            Code = workItem.Code,
            Title = workItem.Title,
            Status = workItem.Status.Name,
            Type = workItem.Type.Name,
            CreatedAt = workItem.CreatedAt,
            UpdatedAt = workItem.UpdatedAt,
        };
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