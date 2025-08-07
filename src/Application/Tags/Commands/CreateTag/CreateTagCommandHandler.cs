using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.CreateTag;

internal sealed class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, ErrorOr<Guid>>
{
    private readonly IProjectsRepository _projectsRepository;

    public CreateTagCommandHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateTagCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithTagsAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var tag = new Tag(
            command.Name,
            command.Description,
            command.Color);

        var result = project.AddTag(tag);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return tag.Id;
    }
}