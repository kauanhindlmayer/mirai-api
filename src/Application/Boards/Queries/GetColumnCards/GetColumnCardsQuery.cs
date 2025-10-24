using Domain.Backlogs;
using ErrorOr;
using MediatR;

namespace Application.Boards.Queries.GetColumnCards;

public sealed record GetColumnCardsQuery(
    Guid BoardId,
    Guid ColumnId,
    BacklogLevel? BacklogLevel = null,
    int PageSize = 20,
    int Page = 1) : IRequest<ErrorOr<ColumnCardsResponse>>;
