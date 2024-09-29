using Application.Common.Interfaces;
using Domain.Projects;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.CreateWikiPage;

public class CreateWikiPageCommandHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<CreateWikiPageCommand, ErrorOr<WikiPage>>
{
    public async Task<ErrorOr<WikiPage>> Handle(
        CreateWikiPageCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithWikiPagesAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        if (request.ParentWikiPageId.HasValue)
        {
            var parentWikiPage = project.WikiPages.FirstOrDefault(wikiPage =>
                wikiPage.Id == request.ParentWikiPageId);

            if (parentWikiPage is null || parentWikiPage.ProjectId != request.ProjectId)
            {
                return WikiPageErrors.ParentWikiPageNotFound;
            }
        }

        var wikiPage = new WikiPage(
            request.ProjectId,
            request.Title,
            request.Content,
            request.ParentWikiPageId);

        var result = project.AddWikiPage(wikiPage);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return wikiPage;
    }
}