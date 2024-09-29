using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.RemoveTag;

public record RemoveTagCommand(Guid WorkItemId, string TagName)
    : IRequest<ErrorOr<Success>>;