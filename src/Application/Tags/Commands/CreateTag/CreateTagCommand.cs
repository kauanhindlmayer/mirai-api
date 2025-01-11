using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.CreateTag;

public sealed record CreateTagCommand(
    Guid ProjectId,
    string Name) : IRequest<ErrorOr<Guid>>;