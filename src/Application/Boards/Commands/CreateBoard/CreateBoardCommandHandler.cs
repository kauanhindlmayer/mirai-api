using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateBoard;

internal sealed class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, ErrorOr<Guid>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IBoardsRepository _boardsRepository;

    public CreateBoardCommandHandler(
        IProjectsRepository projectsRepository,
        IBoardsRepository boardsRepository)
    {
        _projectsRepository = projectsRepository;
        _boardsRepository = boardsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateBoardCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(
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

        await _boardsRepository.AddAsync(board, cancellationToken);

        return board.Id;
    }
}