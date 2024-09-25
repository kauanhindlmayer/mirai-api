using ErrorOr;
using MediatR;
using Mirai.Domain.Tags;

namespace Mirai.Application.Tags.Queries.ListTags;

public record ListTagsQuery(Guid ProjectId) : IRequest<ErrorOr<List<Tag>>>;