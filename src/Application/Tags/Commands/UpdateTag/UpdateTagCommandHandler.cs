using Application.Common.Interfaces;
using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.UpdateTag;

public class UpdateTagCommandHandler(
    IProjectsRepository _projectsRepository)
    : IRequestHandler<UpdateTagCommand, ErrorOr<Tag>>
{
    public async Task<ErrorOr<Tag>> Handle(
        UpdateTagCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithTagsAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        var existingTag = project.Tags.FirstOrDefault(t => t.Id == command.TagId);
        if (existingTag is null)
        {
            return TagErrors.TagNotFound;
        }

        existingTag.UpdateName(command.Name);
        _projectsRepository.Update(project);

        return existingTag;
    }
}