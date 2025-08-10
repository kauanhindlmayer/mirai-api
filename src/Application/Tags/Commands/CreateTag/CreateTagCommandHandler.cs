using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.CreateTag;

internal sealed class CreateTagCommandHandler
    : IRequestHandler<CreateTagCommand, ErrorOr<Guid>>
{
    private readonly IProjectRepository _projectRepository;

    public CreateTagCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateTagCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithTagsAsync(
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

        _projectRepository.Update(project);

        return tag.Id;
    }
}