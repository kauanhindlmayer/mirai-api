using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.AddItem;

public record AddItemCommand(
    string Description,
    Guid RetrospectiveId,
    Guid RetrospectiveColumnId)
    : IRequest<ErrorOr<RetrospectiveItem>>;