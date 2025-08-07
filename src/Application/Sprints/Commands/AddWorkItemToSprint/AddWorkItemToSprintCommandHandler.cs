using Domain.Sprints;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.Sprints.Commands.AddWorkItemToSprint;

internal sealed class AddWorkItemToSprintCommandHandler
    : IRequestHandler<AddWorkItemToSprintCommand, ErrorOr<Success>>
{
    private readonly ISprintsRepository _sprintsRepository;
    private readonly IWorkItemsRepository _workItemsRepository;

    public AddWorkItemToSprintCommandHandler(
        ISprintsRepository sprintsRepository,
        IWorkItemsRepository workItemsRepository)
    {
        _sprintsRepository = sprintsRepository;
        _workItemsRepository = workItemsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddWorkItemToSprintCommand command,
        CancellationToken cancellationToken)
    {
        var sprint = await _sprintsRepository.GetByIdAsync(
            command.SprintId,
            cancellationToken);

        if (sprint is null)
        {
            return SprintErrors.NotFound;
        }

        var workItem = await _workItemsRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var result = sprint.AddWorkItem(workItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        _sprintsRepository.Update(sprint);

        return Result.Success;
    }
}