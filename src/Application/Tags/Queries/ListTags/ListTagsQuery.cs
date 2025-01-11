using ErrorOr;
using MediatR;

namespace Application.Tags.Queries.ListTags;

public sealed record ListTagsQuery(
    Guid ProjectId,
    string? SearchTerm) : IRequest<ErrorOr<List<TagResponse>>>;