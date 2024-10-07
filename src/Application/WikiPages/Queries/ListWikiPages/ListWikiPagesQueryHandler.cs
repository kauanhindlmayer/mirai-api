using Application.Common.Interfaces;
using Domain.Projects;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.ListWikiPages;

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
            return ProjectErrors.NotFound;
        }

        var rootPages = project.WikiPages
            .Where(page => page.ParentWikiPageId is null)
            .ToList();

        return rootPages;
    }
}