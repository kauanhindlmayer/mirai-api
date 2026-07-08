using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.RemovePullRequestLink;

public sealed record RemovePullRequestLinkCommand(
    Guid WorkItemId,
    Guid LinkId) : IRequest<ErrorOr<Success>>;
