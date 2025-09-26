using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.DeleteTags;

internal sealed class DeleteTagsCommandHandler
    : IRequestHandler<DeleteTagsCommand, ErrorOr<Success>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITagRepository _tagsRepository;

    public DeleteTagsCommandHandler(
        IProjectRepository projectRepository,
        ITagRepository tagsRepository)
    {
        _projectRepository = projectRepository;
        _tagsRepository = tagsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteTagsCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithTagsAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var tags = project.Tags
            .Where(tag => command.TagIds.Contains(tag.Id))
            .ToList();

        if (tags.Count == 0)
        {
            return TagErrors.NoTagsFound;
        }

        _tagsRepository.RemoveRange(tags, cancellationToken);
        _projectRepository.Update(project);

        return Result.Success;
    }
}