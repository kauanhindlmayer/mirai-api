using ErrorOr;
using MediatR;
using Mirai.Domain.WorkItems;
using Mirai.Domain.WorkItems.Enums;

namespace Mirai.Application.WorkItems.Commands.CreateWorkItem;

public record CreateWorkItemCommand(Guid ProjectId, WorkItemType Type, string Title)
    : IRequest<ErrorOr<WorkItem>>;