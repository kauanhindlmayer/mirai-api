using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.UpdateTag;

public sealed record UpdateTagCommand(
    Guid ProjectId,
    Guid TagId,
    string Name) : IRequest<ErrorOr<Tag>>;