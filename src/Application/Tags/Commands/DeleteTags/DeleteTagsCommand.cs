using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.DeleteTags;

public sealed record DeleteTagsCommand(
    Guid ProjectId,
    List<Guid> TagIds) : IRequest<ErrorOr<Success>>;