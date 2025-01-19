using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateColumn;

public sealed record CreateColumnCommand(
    Guid BoardId,
    string Name,
    int WipLimit,
    string DefinitionOfDone,
    int Position) : IRequest<ErrorOr<Guid>>;