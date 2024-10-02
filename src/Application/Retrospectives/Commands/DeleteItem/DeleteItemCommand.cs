using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.DeleteItem;

public record DeleteItemCommand(
    Guid RetrospectiveId,
    Guid ColumnId,
    Guid ItemId) : IRequest<ErrorOr<Success>>;