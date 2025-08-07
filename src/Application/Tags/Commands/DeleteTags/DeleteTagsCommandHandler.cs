using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.DeleteTags;

internal sealed class DeleteTagsCommandHandler : IRequestHandler<DeleteTagsCommand, ErrorOr<Success>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly ITagsRepository _tagsRepository;

    public DeleteTagsCommandHandler(
        IProjectsRepository projectsRepository,
        ITagsRepository tagsRepository)
    {
        _projectsRepository = projectsRepository;
        _tagsRepository = tagsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteTagsCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithTagsAsync(
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
        _projectsRepository.Update(project);

        return Result.Success;
    }
}