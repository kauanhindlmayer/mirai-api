using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;

namespace Mirai.Application.Tags.Commands.DeleteTag;

public class DeleteTagCommandHandler(
    IProjectsRepository _projectsRepository,
    ITagsRepository _tagsRepository)
    : IRequestHandler<DeleteTagCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteTagCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithTagsAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        if (await _tagsRepository.IsTagLinkedToAnyWorkItemsAsync(
            project.Id,
            request.TagName,
            cancellationToken))
        {
            return ProjectErrors.TagHasWorkItems;
        }

        project.RemoveTag(request.TagName);
        _projectsRepository.Update(project);

        return Result.Success;
    }
}