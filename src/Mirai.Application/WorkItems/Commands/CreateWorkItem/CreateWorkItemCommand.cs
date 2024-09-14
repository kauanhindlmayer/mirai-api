using ErrorOr;
using MediatR;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.WorkItems.Commands.CreateWorkItem;

public record CreateWorkItemCommand(Guid ProjectId, string Type, string Title)
    : IRequest<ErrorOr<WorkItem>>;