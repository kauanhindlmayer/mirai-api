using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Sprints.Queries.GetIncompleteWorkItems;

/// <summary>
/// Retrieves the flat set of a sprint's incomplete work items - the same set
/// <see cref="Domain.Sprints.Sprint.IncompleteWorkItems"/> returns when the
/// sprint is completed - so callers can require a carry-over disposition for
/// exactly those items, regardless of type or hierarchy position.
/// </summary>
public sealed record GetIncompleteWorkItemsQuery(
    Guid TeamId,
    Guid SprintId) : IAuthorizationRequest<ErrorOr<IReadOnlyList<IncompleteWorkItemResponse>>>
{
    public Permission RequiredPermission => Permission.TeamView;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}