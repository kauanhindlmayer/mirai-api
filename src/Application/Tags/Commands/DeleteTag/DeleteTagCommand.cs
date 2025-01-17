using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.DeleteTag;

public sealed record DeleteTagCommand(
    Guid ProjectId,
    Guid TagId) : IRequest<ErrorOr<Success>>;