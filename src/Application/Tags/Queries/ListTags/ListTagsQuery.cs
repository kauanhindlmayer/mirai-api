using Application.Common;
using ErrorOr;
using MediatR;

namespace Application.Tags.Queries.ListTags;

public sealed record ListTagsQuery(
    Guid ProjectId,
    int Page,
    int PageSize,
    string? SearchTerm) : IRequest<ErrorOr<PaginatedList<TagResponse>>>
{
    public string? SearchTerm { get; set; } = SearchTerm;
}