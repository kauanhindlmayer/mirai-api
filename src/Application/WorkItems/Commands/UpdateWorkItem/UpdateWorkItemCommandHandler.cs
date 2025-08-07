using Application.Abstractions;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.UpdateWorkItem;

internal sealed class UpdateWorkItemCommandHandler
    : IRequestHandler<UpdateWorkItemCommand, ErrorOr<Guid>>
{
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly INlpService _nlpService;

    public UpdateWorkItemCommandHandler(
        IWorkItemsRepository workItemsRepository,
        INlpService nlpService)
    {
        _workItemsRepository = workItemsRepository;
        _nlpService = nlpService;
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
            var embeddingResponse = await _nlpService.GenerateEmbeddingVectorAsync(
                workItem.GetEmbeddingContent(),
                cancellationToken);
            if (embeddingResponse.IsError)
            {
                return embeddingResponse.Errors;
            }

            workItem.SetSearchVector(embeddingResponse.Value);
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