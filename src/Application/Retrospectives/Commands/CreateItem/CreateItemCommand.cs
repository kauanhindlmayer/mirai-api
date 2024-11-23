using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateItem;

public sealed record CreateItemCommand(
    string Description,
    Guid RetrospectiveId,
    Guid RetrospectiveColumnId) : IRequest<ErrorOr<RetrospectiveItem>>;