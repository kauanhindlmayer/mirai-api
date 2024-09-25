using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;
using Mirai.Domain.WorkItems;
using Mirai.Domain.WorkItems.Enums;

namespace Mirai.Application.WorkItems.Commands.CreateWorkItem;

public class CreateWorkItemCommandHandler(
    IProjectsRepository _projectsRepository,
    IWorkItemsRepository _workItemsRepository)
    : IRequestHandler<CreateWorkItemCommand, ErrorOr<WorkItem>>
{
    public async Task<ErrorOr<WorkItem>> Handle(
        CreateWorkItemCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        var workItemCode = await _workItemsRepository.GetNextWorkItemCodeAsync(request.ProjectId, cancellationToken);

        var workItem = new WorkItem(
            projectId: request.ProjectId,
            code: workItemCode,
            title: request.Title,
            type: WorkItemType.UserStory);

        var result = project.AddWorkItem(workItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return workItem;
    }
}