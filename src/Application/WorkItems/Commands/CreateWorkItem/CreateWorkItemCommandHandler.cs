using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Projects;
using Domain.Teams;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.CreateWorkItem;

internal sealed class CreateWorkItemCommandHandler : IRequestHandler<CreateWorkItemCommand, ErrorOr<Guid>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly INlpService _nlpService;

    public CreateWorkItemCommandHandler(
        IProjectsRepository projectsRepository,
        IWorkItemsRepository workItemsRepository,
        INlpService nlpService)
    {
        _projectsRepository = projectsRepository;
        _workItemsRepository = workItemsRepository;
        _nlpService = nlpService;
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

        if (command.AssignedTeamId.HasValue)
        {
            var team = project.Teams.FirstOrDefault(t => t.Id == command.AssignedTeamId);
            if (team is null)
            {
                return TeamErrors.NotFound;
            }
        }

        var workItemCode = await _workItemsRepository.GetNextWorkItemCodeAsync(
            command.ProjectId,
            cancellationToken);

        var workItem = new WorkItem(
            command.ProjectId,
            workItemCode,
            command.Title,
            command.Type,
            command.AssignedTeamId);

        var embeddingResponse = await _nlpService.GenerateEmbeddingVectorAsync(
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