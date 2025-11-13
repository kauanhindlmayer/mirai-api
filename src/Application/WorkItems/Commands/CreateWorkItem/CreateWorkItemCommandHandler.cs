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
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public CreateWorkItemCommandHandler(
        IProjectRepository projectRepository,
        IWorkItemRepository workItemRepository,
        [FromKeyedServices(ServiceKeys.Embeddings)] IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        _projectRepository = projectRepository;
        _workItemRepository = workItemRepository;
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithTeamsAsync(
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

        var workItemCode = await _workItemRepository.GetNextWorkItemCodeAsync(
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

        _projectRepository.Update(project);

        return workItem.Id;
    }
}