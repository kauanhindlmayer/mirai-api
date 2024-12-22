using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Projects;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.CreateWikiPage;

internal sealed class CreateWikiPageCommandHandler(
    IProjectsRepository projectsRepository,
    IUserContext userContext)
    : IRequestHandler<CreateWikiPageCommand, ErrorOr<WikiPage>>
{
    public async Task<ErrorOr<WikiPage>> Handle(
        CreateWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var project = await projectsRepository.GetByIdWithWikiPagesAsync(
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
            userContext.UserId,
            command.ParentWikiPageId);

        var result = project.AddWikiPage(wikiPage);
        if (result.IsError)
        {
            return result.Errors;
        }

        projectsRepository.Update(project);

        return wikiPage;
    }
}