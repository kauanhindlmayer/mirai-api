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
        ListTagsQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        var tags = await _tagsRepository.GetByProjectAsync(
            request.ProjectId,
            request.SearchTerm,
            cancellationToken);

        return tags;
    }
}