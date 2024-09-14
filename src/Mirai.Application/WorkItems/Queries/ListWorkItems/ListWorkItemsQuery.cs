using ErrorOr;
using MediatR;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.WorkItems.Queries.ListWorkItems;

public record ListWorkItemsQuery(Guid ProjectId) : IRequest<ErrorOr<List<WorkItem>>>;