using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;
using Mirai.Domain.Tags;

namespace Mirai.Application.Tags.Commands.UpdateTag;

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