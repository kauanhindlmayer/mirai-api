using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateBoard;

internal sealed class CreateBoardCommandHandler(
    IProjectsRepository projectRepository,
    IBoardsRepository boardRepository)
    : IRequestHandler<CreateBoardCommand, ErrorOr<Board>>
{
    public async Task<ErrorOr<Board>> Handle(
        CreateBoardCommand command,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var board = new Board(
            project.Id,
            command.Name,
            command.Description);

        await boardRepository.AddAsync(board, cancellationToken);

        return board;
    }
}