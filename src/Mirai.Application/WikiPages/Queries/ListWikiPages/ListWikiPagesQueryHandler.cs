using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WikiPages;

namespace Mirai.Application.WikiPages.Queries.ListWikiPages;

public class ListWikiPagesQueryHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<ListWikiPagesQuery, ErrorOr<List<WikiPage>>>
{
    public async Task<ErrorOr<List<WikiPage>>> Handle(
        ListWikiPagesQuery query,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Error.NotFound(description: "Project not found");
        }

        var rootPages = project.WikiPages
            .Where(page => page.ParentWikiPageId is null)
            .ToList();

        return rootPages;
    }
}