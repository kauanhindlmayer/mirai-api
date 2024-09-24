using ErrorOr;
using MediatR;
using Mirai.Domain.Tags;

namespace Mirai.Application.Projects.Commands.CreateTag;

public record CreateTagCommand(Guid ProjectId, string Name)
    : IRequest<ErrorOr<Tag>>;