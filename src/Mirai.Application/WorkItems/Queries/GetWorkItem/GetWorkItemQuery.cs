using ErrorOr;
using MediatR;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.WorkItems.Queries.GetWorkItem;

public record GetWorkItemQuery(Guid WorkItemId) : IRequest<ErrorOr<WorkItem>>;