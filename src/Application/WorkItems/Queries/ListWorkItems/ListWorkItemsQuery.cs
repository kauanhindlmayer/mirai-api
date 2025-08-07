using Application.Abstractions;
using Domain.WorkItems.Enums;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.ListWorkItems;

public sealed record ListWorkItemsQuery(
    Guid ProjectId,
    int Page,
    int PageSize,
    string? Sort,
    string? SearchTerm,
    WorkItemType? Type,
    WorkItemStatus? Status,
    Guid? AssigneeId) : IRequest<ErrorOr<PaginatedList<WorkItemBriefResponse>>>
{
    public string? SearchTerm { get; set; } = SearchTerm;
}