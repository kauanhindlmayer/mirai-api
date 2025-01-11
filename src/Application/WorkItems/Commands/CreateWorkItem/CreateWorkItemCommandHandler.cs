using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Projects;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.CreateWorkItem;

internal sealed class CreateWorkItemCommandHandler : IRequestHandler<CreateWorkItemCommand, ErrorOr<Guid>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly IEmbeddingService _embeddingService;

    public CreateWorkItemCommandHandler(
        IProjectsRepository projectsRepository,
        IWorkItemsRepository workItemsRepository,
        IEmbeddingService embeddingService)
    {
        _projectsRepository = projectsRepository;
        _workItemsRepository = workItemsRepository;
        _embeddingService = embeddingService;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var workItemCode = await _workItemsRepository.GetNextWorkItemCodeAsync(
            command.ProjectId,
            cancellationToken);

        var workItem = new WorkItem(
            command.ProjectId,
            workItemCode,
            command.Title,
            command.Type);

        var embeddingResponse = await _embeddingService.GenerateEmbeddingAsync(
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

        _projectsRepository.Update(project);

        return workItem.Id;
    }
}