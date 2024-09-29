using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.ListWorkItems;

public record ListWorkItemsQuery(Guid ProjectId) : IRequest<ErrorOr<List<WorkItem>>>;