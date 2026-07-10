using Application.Abstractions;
using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Tags.Queries.ListTags;

public sealed record ListTagsQuery(
    Guid ProjectId,
    int Page,
    int PageSize,
    string? Sort,
    string? SearchTerm) : IAuthorizationRequest<ErrorOr<PaginatedList<TagResponse>>>
{
    public string? SearchTerm { get; set; } = SearchTerm;

    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
