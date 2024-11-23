using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AddTag;

public sealed record AddTagCommand(
    Guid WorkItemId,
    string TagName) : IRequest<ErrorOr<Success>>;