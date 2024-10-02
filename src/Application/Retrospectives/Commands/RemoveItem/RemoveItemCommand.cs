using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.RemoveItem;

public record RemoveItemCommand(
    Guid RetrospectiveId,
    Guid ColumnId,
    Guid ItemId) : IRequest<ErrorOr<Success>>;