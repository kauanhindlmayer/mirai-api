using Application.Abstractions;
using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Queries.GetMentionableProjectUsers;

public sealed record GetMentionableProjectUsersQuery(
    Guid ProjectId,
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null) : IAuthorizationRequest<ErrorOr<PaginatedList<MentionableProjectUserResponse>>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
