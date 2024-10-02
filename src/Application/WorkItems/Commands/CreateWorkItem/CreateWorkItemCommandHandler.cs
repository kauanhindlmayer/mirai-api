using Application.Common.Interfaces;
using Domain.Projects;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.CreateWorkItem;

public class CreateWorkItemCommandHandler(
    IProjectsRepository _projectsRepository,
    IWorkItemsRepository _workItemsRepository)
    : IRequestHandler<CreateWorkItemCommand, ErrorOr<WorkItem>>
{
    public async Task<ErrorOr<WorkItem>> Handle(
        CreateWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        var workItemCode = await _workItemsRepository.GetNextWorkItemCodeAsync(
            command.ProjectId,
            cancellationToken);

        var workItem = new WorkItem(
            command.ProjectId,
            workItemCode,
            command.Title,
            command.Type);

        var result = project.AddWorkItem(workItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return workItem;
    }
}