using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospectiveItem;

public sealed record CreateRetrospectiveItemCommand(
    string Description,
    Guid RetrospectiveId,
    Guid ColumnId) : IRequest<ErrorOr<RetrospectiveItem>>;