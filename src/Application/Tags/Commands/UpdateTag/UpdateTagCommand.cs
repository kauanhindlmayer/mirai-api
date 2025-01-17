using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.UpdateTag;

public sealed record UpdateTagCommand(
    Guid ProjectId,
    Guid TagId,
    string Name,
    string Description,
    string Color) : IRequest<ErrorOr<Guid>>;