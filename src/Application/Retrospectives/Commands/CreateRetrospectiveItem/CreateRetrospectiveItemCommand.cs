using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospectiveItem;

public sealed record CreateRetrospectiveItemCommand(
    string Content,
    Guid RetrospectiveId,
    Guid ColumnId) : IRequest<ErrorOr<RetrospectiveItem>>;