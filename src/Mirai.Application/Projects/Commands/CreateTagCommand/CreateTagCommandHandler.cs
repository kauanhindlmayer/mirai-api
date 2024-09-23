using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.Projects.Commands.CreateTagCommand;

public class CreateTagCommandHandler(
    IProjectsRepository _projectsRepository,
    ITagsRepository _tagsRepository)
    : IRequestHandler<CreateTagCommand, ErrorOr<Tag>>
{
    public async Task<ErrorOr<Tag>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        // TODO: Refactor the tag to make it belong to a project and not be global.
        var tag = new Tag(request.Name);
        await _tagsRepository.AddAsync(tag, cancellationToken);

        return tag;
    }
}