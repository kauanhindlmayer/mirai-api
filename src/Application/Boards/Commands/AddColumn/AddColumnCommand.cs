using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.AddColumn;

public record AddColumnCommand(
    Guid BoardId,
    string Name,
    int WipLimit,
    string DefinitionOfDone) : IRequest<ErrorOr<BoardColumn>>;