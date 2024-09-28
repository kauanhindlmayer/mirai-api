using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Boards;
using Mirai.Domain.Projects;

namespace Mirai.Application.Boards.Queries.ListBoards;

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