using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;
using Mirai.Domain.Tags;

namespace Mirai.Application.Tags.Queries.ListTags;

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