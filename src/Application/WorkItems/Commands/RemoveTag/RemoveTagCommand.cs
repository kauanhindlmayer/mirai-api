using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.RemoveTag;

public sealed record RemoveTagCommand(
    Guid WorkItemId,
    string TagName) : IRequest<ErrorOr<Success>>;