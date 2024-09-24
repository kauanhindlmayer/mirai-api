using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;
using Mirai.Domain.WikiPages;

namespace Mirai.Application.WikiPages.Queries.ListWikiPages;

public class ListWikiPagesQueryHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<ListWikiPagesQuery, ErrorOr<List<WikiPage>>>
{
    public async Task<ErrorOr<List<WikiPage>>> Handle(
        ListWikiPagesQuery query,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithWikiPagesAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        var rootPages = project.WikiPages
            .Where(page => page.ParentWikiPageId is null)
            .ToList();

        return rootPages;
    }
}