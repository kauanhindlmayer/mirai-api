using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.DeleteTag;

internal sealed class DeleteTagCommandHandler(
    IProjectsRepository projectsRepository,
    ITagsRepository tagsRepository)
    : IRequestHandler<DeleteTagCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteTagCommand command,
        CancellationToken cancellationToken)
    {
        var project = await projectsRepository.GetByIdWithTagsAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        if (await tagsRepository.IsTagLinkedToAnyWorkItemsAsync(
            project.Id,
            command.TagName,
            cancellationToken))
        {
            return ProjectErrors.TagHasWorkItems;
        }

        project.RemoveTag(command.TagName);
        projectsRepository.Update(project);

        return Result.Success;
    }
}