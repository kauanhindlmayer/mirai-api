using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Queries.ListTags;

public record ListTagsQuery(Guid ProjectId, string? SearchTerm)
    : IRequest<ErrorOr<List<Tag>>>;