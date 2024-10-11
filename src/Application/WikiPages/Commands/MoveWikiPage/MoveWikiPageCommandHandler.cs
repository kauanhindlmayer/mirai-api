using Application.Common.Interfaces;
using Domain.Projects;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.MoveWikiPage;

public class MoveWikiPageCommandHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<MoveWikiPageCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        MoveWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithWikiPagesAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var result = project.MoveWikiPage(
            command.WikiPageId,
            command.TargetParentId,
            command.TargetPosition);

        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return Result.Success;
    }
}
