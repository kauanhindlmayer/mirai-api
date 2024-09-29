using Application.Common.Interfaces;
using Domain.Boards;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Boards.Queries.ListBoards;

public class ListBoardsQueryHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<ListBoardsQuery, ErrorOr<List<Board>>>
{
    public async Task<ErrorOr<List<Board>>> Handle(
        ListBoardsQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithBoardsAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        return project.Boards.ToList();
    }
}