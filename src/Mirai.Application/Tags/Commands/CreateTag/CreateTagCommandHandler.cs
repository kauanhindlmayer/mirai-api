using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;
using Mirai.Domain.Tags;

namespace Mirai.Application.Tags.Commands.CreateTag;

public class CreateTagCommandHandler(
    IProjectsRepository _projectsRepository)
    : IRequestHandler<CreateTagCommand, ErrorOr<Tag>>
{
    public async Task<ErrorOr<Tag>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithTagsAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        var tag = new Tag(request.Name);

        var result = project.AddTag(tag);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return tag;
    }
}