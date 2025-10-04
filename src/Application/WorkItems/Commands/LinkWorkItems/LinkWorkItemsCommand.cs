using Domain.WorkItems.Enums;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.LinkWorkItems;

public sealed record LinkWorkItemsCommand(
    Guid SourceWorkItemId,
    Guid TargetWorkItemId,
    WorkItemLinkType LinkType,
    string? Comment) : IRequest<ErrorOr<Guid>>;
