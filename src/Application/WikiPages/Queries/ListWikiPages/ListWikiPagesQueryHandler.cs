using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.ListWikiPages;

internal sealed class ListWikiPagesQueryHandler(IProjectsRepository projectsRepository)
    : IRequestHandler<ListWikiPagesQuery, ErrorOr<List<WikiPage>>>
{
    public async Task<ErrorOr<List<WikiPage>>> Handle(
        ListWikiPagesQuery query,
        CancellationToken cancellationToken)
    {
        var project = await projectsRepository.GetByIdWithWikiPagesAsync(
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