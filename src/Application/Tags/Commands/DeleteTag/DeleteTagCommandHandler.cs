using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.DeleteTag;

internal sealed class DeleteTagCommandHandler
    : IRequestHandler<DeleteTagCommand, ErrorOr<Success>>
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

        var result = project.RemoveTag(command.TagId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _tagsRepository.Remove(result.Value);
        _projectsRepository.Update(project);

        return Result.Success;
    }
}