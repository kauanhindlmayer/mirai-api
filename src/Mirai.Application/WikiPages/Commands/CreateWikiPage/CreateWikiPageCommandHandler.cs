using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WikiPages;

namespace Mirai.Application.WikiPages.Commands.CreateWikiPage;

public class CreateWikiPageCommandHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<CreateWikiPageCommand, ErrorOr<WikiPage>>
{
    public async Task<ErrorOr<WikiPage>> Handle(
        CreateWikiPageCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Error.NotFound(description: "Project not found");
        }

        if (request.ParentWikiPageId.HasValue)
        {
            var parentWikiPage = project.WikiPages.FirstOrDefault(wikiPage => wikiPage.Id == request.ParentWikiPageId);
            if (parentWikiPage is null)
            {
                return Error.NotFound(description: "Parent Wiki Page not found");
            }
        }

        var wikiPage = new WikiPage(
            projectId: request.ProjectId,
            title: request.Title,
            content: request.Content,
            parentWikiPageId: request.ParentWikiPageId);

        var result = project.AddWikiPage(wikiPage);
        if (result.IsError)
        {
            return result.Errors;
        }

        await _projectsRepository.UpdateAsync(project, cancellationToken);

        return wikiPage;
    }
}