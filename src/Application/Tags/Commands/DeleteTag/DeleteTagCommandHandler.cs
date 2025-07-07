using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using Domain.Tags;
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

        var tag = project.Tags.FirstOrDefault(t => t.Id == command.TagId);
        if (tag is null)
        {
            return TagErrors.NotFound;
        }

        var result = project.RemoveTag(tag);
        if (result.IsError)
        {
            return result.Errors;
        }

        _tagsRepository.Remove(tag);
        _projectsRepository.Update(project);

        return Result.Success;
    }
}