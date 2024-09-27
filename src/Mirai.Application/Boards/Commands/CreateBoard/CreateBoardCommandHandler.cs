using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Boards;
using Mirai.Domain.Projects;

namespace Mirai.Application.Boards.Commands.CreateBoard;

public class CreateBoardCommandHandler(
    IProjectsRepository _projectRepository,
    IBoardsRepository _boardRepository)
    : IRequestHandler<CreateBoardCommand, ErrorOr<Board>>
{
    public async Task<ErrorOr<Board>> Handle(
        CreateBoardCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        var board = new Board(
            project.Id,
            command.Name,
            command.Description);

        await _boardRepository.AddAsync(board, cancellationToken);

        return board;
    }
}