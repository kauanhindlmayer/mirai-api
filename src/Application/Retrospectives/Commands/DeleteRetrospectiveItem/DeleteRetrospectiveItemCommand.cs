using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.DeleteRetrospectiveItem;

public sealed record DeleteRetrospectiveItemCommand(
    Guid RetrospectiveId,
    Guid ColumnId,
    Guid ItemId) : IRequest<ErrorOr<Success>>;