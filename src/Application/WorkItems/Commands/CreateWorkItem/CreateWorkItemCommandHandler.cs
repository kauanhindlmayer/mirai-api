using Domain.Projects;
using Domain.Shared;
using Domain.Teams;
using Domain.WorkItems;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Application.WorkItems.Commands.CreateWorkItem;

internal sealed class CreateWorkItemCommandHandler
    : IRequestHandler<CreateWorkItemCommand, ErrorOr<Guid>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public CreateWorkItemCommandHandler(
        IProjectsRepository projectsRepository,
        IWorkItemsRepository workItemsRepository,
        [FromKeyedServices(ServiceKeys.Embedding)] IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        _projectsRepository = projectsRepository;
        _workItemsRepository = workItemsRepository;
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithTeamsAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var team = project.Teams.FirstOrDefault(t => t.Id == command.AssignedTeamId);
        if (team is null)
        {
            return TeamErrors.NotFound;
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

        var embedding = await _embeddingGenerator.GenerateAsync(
            workItem.GetEmbeddingContent(),
            cancellationToken: cancellationToken);

        workItem.SetSearchVector(embedding.Vector.ToArray());

        var result = project.AddWorkItem(workItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return workItem.Id;
    }
}