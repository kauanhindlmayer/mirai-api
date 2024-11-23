using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.MoveWikiPage;

internal sealed class MoveWikiPageCommandHandler(IProjectsRepository projectsRepository)
    : IRequestHandler<MoveWikiPageCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        MoveWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var project = await projectsRepository.GetByIdWithWikiPagesAsync(
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

        projectsRepository.Update(project);

        return Result.Success;
    }
}
