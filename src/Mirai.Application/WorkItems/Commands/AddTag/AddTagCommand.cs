using ErrorOr;
using MediatR;

namespace Mirai.Application.WorkItems.Commands.AddTag;

public record AddTagCommand(Guid WorkItemId, string TagName)
    : IRequest<ErrorOr<Success>>;