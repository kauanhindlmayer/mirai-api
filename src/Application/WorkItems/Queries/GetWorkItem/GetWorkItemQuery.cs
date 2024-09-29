using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.GetWorkItem;

public record GetWorkItemQuery(Guid WorkItemId) : IRequest<ErrorOr<WorkItem>>;