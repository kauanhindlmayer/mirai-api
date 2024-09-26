using ErrorOr;
using MediatR;
using Mirai.Domain.Tags;

namespace Mirai.Application.Tags.Queries.ListTags;

public record ListTagsQuery(Guid ProjectId, string? SearchTerm)
    : IRequest<ErrorOr<List<Tag>>>;