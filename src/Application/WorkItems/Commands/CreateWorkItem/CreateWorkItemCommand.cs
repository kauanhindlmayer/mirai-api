using Domain.WorkItems.Enums;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.CreateWorkItem;

public sealed record CreateWorkItemCommand(
    Guid ProjectId,
    WorkItemType Type,
    string Title) : IRequest<ErrorOr<Guid>>;