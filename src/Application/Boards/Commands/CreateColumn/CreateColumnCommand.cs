using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateColumn;

public record CreateColumnCommand(
    Guid BoardId,
    string Name,
    int WipLimit,
    string DefinitionOfDone) : IRequest<ErrorOr<BoardColumn>>;