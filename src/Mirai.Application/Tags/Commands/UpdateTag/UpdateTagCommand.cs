using ErrorOr;
using MediatR;
using Mirai.Domain.Tags;

namespace Mirai.Application.Tags.Commands.UpdateTag;

public record UpdateTagCommand(Guid ProjectId, Guid TagId, string Name)
    : IRequest<ErrorOr<Tag>>;