using Application.Common.Interfaces;
using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.CreateTag;

public class CreateTagCommandHandler(
    IProjectsRepository _projectsRepository)
    : IRequestHandler<CreateTagCommand, ErrorOr<Tag>>
{
    public async Task<ErrorOr<Tag>> Handle(
        CreateTagCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithTagsAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        var tag = new Tag(command.Name);

        var result = project.AddTag(tag);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return tag;
    }
}