using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WorkItems.Queries.SearchWorkItems;

public sealed record SearchWorkItemsQuery(
    Guid ProjectId,
    string SearchTerm) : IAuthorizationRequest<ErrorOr<IReadOnlyList<WorkItemResponseWithDistance>>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
