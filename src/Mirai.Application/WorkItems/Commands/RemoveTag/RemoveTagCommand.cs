using ErrorOr;
using MediatR;

namespace Mirai.Application.WorkItems.Commands.RemoveTag;

public record RemoveTagCommand(Guid WorkItemId, string TagName)
    : IRequest<ErrorOr<Success>>;