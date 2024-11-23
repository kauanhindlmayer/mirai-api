using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.DeleteTag;

public sealed record DeleteTagCommand(
    Guid ProjectId,
    string TagName) : IRequest<ErrorOr<Success>>;