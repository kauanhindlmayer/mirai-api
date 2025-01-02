using Application.Common;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.ListWorkItems;

public sealed record ListWorkItemsQuery(
    Guid ProjectId,
    int PageNumber,
    int PageSize,
    string? SortField,
    string? SortOrder,
    string? SearchTerm) : IRequest<ErrorOr<PaginatedList<WorkItem>>>;