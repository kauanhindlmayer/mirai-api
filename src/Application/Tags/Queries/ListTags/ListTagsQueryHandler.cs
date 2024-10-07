using Application.Common.Interfaces;
using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Queries.ListTags;

public class ListTagsQueryHandler(
    IProjectsRepository _projectsRepository,
    ITagsRepository _tagsRepository)
    : IRequestHandler<ListTagsQuery, ErrorOr<List<Tag>>>
{
    public async Task<ErrorOr<List<Tag>>> Handle(
        ListTagsQuery query,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var tags = await _tagsRepository.GetByProjectAsync(
            query.ProjectId,
            query.SearchTerm,
            cancellationToken);

        return tags;
    }
}