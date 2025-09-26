using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.DeleteTag;

internal sealed class DeleteTagCommandHandler
    : IRequestHandler<DeleteTagCommand, ErrorOr<Success>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITagRepository _tagsRepository;

    public DeleteTagCommandHandler(
        IProjectRepository projectRepository,
        ITagRepository tagsRepository)
    {
        _projectRepository = projectRepository;
        _tagsRepository = tagsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteTagCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithTagsAsync(
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
        _projectRepository.Update(project);

        return Result.Success;
    }
}