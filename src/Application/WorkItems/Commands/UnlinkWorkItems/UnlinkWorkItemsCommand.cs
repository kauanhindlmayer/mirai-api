using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.UnlinkWorkItems;

public sealed record UnlinkWorkItemsCommand(
    Guid WorkItemId,
    Guid LinkId) : IRequest<ErrorOr<Success>>;
