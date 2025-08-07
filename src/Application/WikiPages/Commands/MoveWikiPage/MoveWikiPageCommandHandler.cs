using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.MoveWikiPage;

internal sealed class MoveWikiPageCommandHandler : IRequestHandler<MoveWikiPageCommand, ErrorOr<Success>>
{
    private readonly IProjectsRepository _projectsRepository;

    public MoveWikiPageCommandHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

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
