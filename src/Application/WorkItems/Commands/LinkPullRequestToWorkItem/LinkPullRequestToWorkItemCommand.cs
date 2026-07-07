using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.LinkPullRequestToWorkItem;

public sealed record LinkPullRequestToWorkItemCommand(
    Guid WorkItemId,
    int PullRequestNumber) : IRequest<ErrorOr<Guid>>;
