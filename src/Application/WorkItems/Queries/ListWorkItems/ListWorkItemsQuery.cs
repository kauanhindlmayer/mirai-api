using Application.Common;
using Application.WorkItems.Queries.SearchWorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.ListWorkItems;

public sealed record ListWorkItemsQuery(
    Guid ProjectId,
    int PageNumber,
    int PageSize,
    string? SortField,
    string? SortOrder,
    string? SearchTerm) : IRequest<ErrorOr<PaginatedList<WorkItemBriefResponse>>>;