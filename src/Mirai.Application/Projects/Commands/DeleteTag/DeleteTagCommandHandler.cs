using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;
using Mirai.Domain.Tags;

namespace Mirai.Application.Projects.Commands.DeleteTag;

public class DeleteTagCommandHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<DeleteTagCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteTagCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        project.RemoveTag(request.TagName);
        await _projectsRepository.UpdateAsync(project, cancellationToken);

        return Result.Success;
    }
}