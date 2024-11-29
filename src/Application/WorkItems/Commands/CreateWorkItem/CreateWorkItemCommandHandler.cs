using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Projects;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.CreateWorkItem;

internal sealed class CreateWorkItemCommandHandler(
    IProjectsRepository projectsRepository,
    IWorkItemsRepository workItemsRepository,
    IEmbeddingService embeddingService)
    : IRequestHandler<CreateWorkItemCommand, ErrorOr<WorkItem>>
{
    public async Task<ErrorOr<WorkItem>> Handle(
        CreateWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var project = await projectsRepository.GetByIdAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var workItemCode = await workItemsRepository.GetNextWorkItemCodeAsync(
            command.ProjectId,
            cancellationToken);

        var workItem = new WorkItem(
            command.ProjectId,
            workItemCode,
            command.Title,
            command.Type);

        var embeddingResponse = await embeddingService.GenerateEmbeddingAsync(
            $"{workItem.Title} {workItem.Description}");
        if (embeddingResponse.IsError)
        {
            return embeddingResponse.Errors;
        }

        workItem.SetSearchVector(embeddingResponse.Value);

        var result = project.AddWorkItem(workItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        projectsRepository.Update(project);

        return workItem;
    }
}