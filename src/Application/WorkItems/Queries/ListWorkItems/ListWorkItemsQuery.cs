using Application.Abstractions;
using Application.Abstractions.Authorization;
using Domain.Authorization;
using Domain.WorkItems.Enums;
using ErrorOr;

namespace Application.WorkItems.Queries.ListWorkItems;

public sealed record ListWorkItemsQuery(
    Guid ProjectId,
    int Page,
    int PageSize,
    string? Sort,
    string? SearchTerm,
    WorkItemType? Type,
    WorkItemStatus? Status,
    Guid? AssigneeId) : IAuthorizationRequest<ErrorOr<PaginatedList<WorkItemBriefResponse>>>
{
    public string? SearchTerm { get; set; } = SearchTerm;

    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
