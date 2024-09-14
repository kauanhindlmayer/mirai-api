using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.WorkItems.Commands.CreateWorkItem;

public class CreateWorkItemCommandHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<CreateWorkItemCommand, ErrorOr<WorkItem>>
{
    public async Task<ErrorOr<WorkItem>> Handle(
        CreateWorkItemCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Error.NotFound(description: "Project not found");
        }

        var workItem = new WorkItem(
            projectId: request.ProjectId,
            type: WorkItemType.UserStory,
            title: request.Title);

        var result = project.AddWorkItem(workItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        await _projectsRepository.UpdateAsync(project, cancellationToken);

        return workItem;
    }
}