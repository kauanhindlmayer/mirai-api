using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Tags.Commands.CreateTag;

internal sealed class CreateTagCommandHandler(
    IProjectsRepository projectsRepository)
    : IRequestHandler<CreateTagCommand, ErrorOr<Tag>>
{
    public async Task<ErrorOr<Tag>> Handle(
        CreateTagCommand command,
        CancellationToken cancellationToken)
    {
        var project = await projectsRepository.GetByIdWithTagsAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var tag = new Tag(command.Name);

        var result = project.AddTag(tag);
        if (result.IsError)
        {
            return result.Errors;
        }

        projectsRepository.Update(project);

        return tag;
    }
}