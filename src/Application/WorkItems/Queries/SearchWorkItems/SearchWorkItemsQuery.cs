using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.SearchWorkItems;

public sealed record SearchWorkItemsQuery(
    Guid ProjectId,
    string SearchTerm) : IRequest<ErrorOr<List<WorkItem>>>;