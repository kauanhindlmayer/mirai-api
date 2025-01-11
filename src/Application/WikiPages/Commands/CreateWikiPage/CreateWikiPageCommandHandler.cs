using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Projects;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.CreateWikiPage;

internal sealed class CreateWikiPageCommandHandler : IRequestHandler<CreateWikiPageCommand, ErrorOr<Guid>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IUserContext _userContext;

    public CreateWikiPageCommandHandler(
        IProjectsRepository projectsRepository,
        IUserContext userContext)
    {
        _projectsRepository = projectsRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithWikiPagesAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        if (command.ParentWikiPageId.HasValue)
        {
            var parentWikiPage = project.WikiPages.FirstOrDefault(wikiPage =>
                wikiPage.Id == command.ParentWikiPageId);

            if (parentWikiPage is null || parentWikiPage.ProjectId != command.ProjectId)
            {
                return WikiPageErrors.ParentWikiPageNotFound;
            }
        }

        var wikiPage = new WikiPage(
            command.ProjectId,
            command.Title,
            command.Content,
            _userContext.UserId,
            command.ParentWikiPageId);

        var result = project.AddWikiPage(wikiPage);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return wikiPage.Id;
    }
}