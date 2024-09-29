using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.UpdateTag;

public record UpdateTagCommand(Guid ProjectId, Guid TagId, string Name)
    : IRequest<ErrorOr<Tag>>;