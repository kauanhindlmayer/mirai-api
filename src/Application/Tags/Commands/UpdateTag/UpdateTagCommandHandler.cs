using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.UpdateTag;

internal sealed class UpdateTagCommandHandler
    : IRequestHandler<UpdateTagCommand, ErrorOr<Guid>>
{
    private readonly IProjectRepository _projectRepository;

    public UpdateTagCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        UpdateTagCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithTagsAsync(
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

        tag.Update(command.Name, command.Description, command.Color);
        _projectRepository.Update(project);

        return tag.Id;
    }
}