using Domain.Shared;
using Domain.WorkItems;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Application.WorkItems.Commands.UpdateWorkItem;

internal sealed class UpdateWorkItemCommandHandler
    : IRequestHandler<UpdateWorkItemCommand, ErrorOr<Guid>>
{
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public UpdateWorkItemCommandHandler(
        IWorkItemsRepository workItemsRepository,
        [FromKeyedServices(ServiceKeys.Embedding)] IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        _workItemsRepository = workItemsRepository;
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<ErrorOr<Guid>> Handle(
        UpdateWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemsRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        workItem.Update(
            command.Type,
            command.Title,
            command.Description,
            command.AcceptanceCriteria,
            command.Status,
            command.AssigneeId,
            command.AssignedTeamId,
            command.SprintId,
            command.ParentWorkItemId,
            command.Planning,
            command.Classification);

        if (HasWorkItemContentChanged(command, workItem))
        {
            var embedding = await _embeddingGenerator.GenerateAsync(
                workItem.GetEmbeddingContent(),
                cancellationToken: cancellationToken);

            workItem.SetSearchVector(embedding.Vector.ToArray());
        }

        _workItemsRepository.Update(workItem);

        return workItem.Id;
    }

    private static bool HasWorkItemContentChanged(UpdateWorkItemCommand command, WorkItem workItem)
    {
        return (command.Title is not null && command.Title != workItem.Title) ||
               (command.Description is not null && command.Description != workItem.Description) ||
               (command.AcceptanceCriteria is not null && command.AcceptanceCriteria != workItem.AcceptanceCriteria);
    }
}