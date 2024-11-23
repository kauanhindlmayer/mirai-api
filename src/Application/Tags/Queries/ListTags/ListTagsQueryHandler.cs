using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Queries.ListTags;

internal sealed class ListTagsQueryHandler(
    IProjectsRepository projectsRepository,
    ITagsRepository tagsRepository)
    : IRequestHandler<ListTagsQuery, ErrorOr<List<Tag>>>
{
    public async Task<ErrorOr<List<Tag>>> Handle(
        ListTagsQuery query,
        CancellationToken cancellationToken)
    {
        var project = await projectsRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var tags = await tagsRepository.GetByProjectAsync(
            query.ProjectId,
            query.SearchTerm,
            cancellationToken);

        return tags;
    }
}