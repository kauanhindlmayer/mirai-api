using Domain.Sprints;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.Sprints.Commands.AddWorkItemToSprint;

internal sealed class AddWorkItemToSprintCommandHandler
    : IRequestHandler<AddWorkItemToSprintCommand, ErrorOr<Success>>
{
    private readonly ISprintRepository _sprintRepository;
    private readonly IWorkItemRepository _workItemRepository;

    public AddWorkItemToSprintCommandHandler(
        ISprintRepository sprintRepository,
        IWorkItemRepository workItemRepository)
    {
        _sprintRepository = sprintRepository;
        _workItemRepository = workItemRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddWorkItemToSprintCommand command,
        CancellationToken cancellationToken)
    {
        var sprint = await _sprintRepository.GetByIdAsync(
            command.SprintId,
            cancellationToken);

        if (sprint is null)
        {
            return SprintErrors.NotFound;
        }

        var workItem = await _workItemRepository.GetByIdAsync(
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

        _sprintRepository.Update(sprint);

        return Result.Success;
    }
}