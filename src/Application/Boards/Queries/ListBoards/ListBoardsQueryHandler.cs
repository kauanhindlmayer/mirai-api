using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Boards.Queries.ListBoards;

internal sealed class ListBoardsQueryHandler(IProjectsRepository projectsRepository)
    : IRequestHandler<ListBoardsQuery, ErrorOr<List<Board>>>
{
    public async Task<ErrorOr<List<Board>>> Handle(
        ListBoardsQuery query,
        CancellationToken cancellationToken)
    {
        var project = await projectsRepository.GetByIdWithBoardsAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        return project.Boards.ToList();
    }
}