using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.DeleteTag;

internal sealed class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, ErrorOr<Success>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly ITagsRepository _tagsRepository;

    public DeleteTagCommandHandler(
        IProjectsRepository projectsRepository,
        ITagsRepository tagsRepository)
    {
        _projectsRepository = projectsRepository;
        _tagsRepository = tagsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteTagCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithTagsAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        if (await _tagsRepository.IsTagLinkedToAnyWorkItemsAsync(
            project.Id,
            command.TagName,
            cancellationToken))
        {
            return ProjectErrors.TagHasWorkItems;
        }

        project.RemoveTag(command.TagName);
        _projectsRepository.Update(project);

        return Result.Success;
    }
}