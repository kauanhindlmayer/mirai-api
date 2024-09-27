using ErrorOr;
using MediatR;
using Mirai.Domain.Boards;

namespace Mirai.Application.Boards.Commands.AddColumn;

public record AddColumnCommand(
    Guid BoardId,
    string Name,
    int WipLimit,
    string DefinitionOfDone) : IRequest<ErrorOr<BoardColumn>>;