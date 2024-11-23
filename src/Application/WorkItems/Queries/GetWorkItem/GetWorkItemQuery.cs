using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.GetWorkItem;

public sealed record GetWorkItemQuery(Guid WorkItemId) : IRequest<ErrorOr<WorkItem>>;