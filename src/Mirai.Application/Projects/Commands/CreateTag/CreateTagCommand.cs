using ErrorOr;
using MediatR;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.Projects.Commands.CreateTag;

public record CreateTagCommand(Guid ProjectId, string Name)
    : IRequest<ErrorOr<Tag>>;