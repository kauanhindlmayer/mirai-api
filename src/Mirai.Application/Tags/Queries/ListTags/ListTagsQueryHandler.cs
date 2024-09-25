using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;
using Mirai.Domain.Tags;

namespace Mirai.Application.Tags.Queries.ListTags;

public class ListTagsQueryHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<ListTagsQuery, ErrorOr<List<Tag>>>
{
    public async Task<ErrorOr<List<Tag>>> Handle(
        ListTagsQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithTagsAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        return project.Tags.ToList();
    }
}