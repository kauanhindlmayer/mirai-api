using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.DeleteRetrospectiveItem;

internal sealed class DeleteRetrospectiveItemCommandHandler
    : IRequestHandler<DeleteRetrospectiveItemCommand, ErrorOr<Success>>
{
    private readonly IRetrospectivesRepository _retrospectivesRepository;

    public DeleteRetrospectiveItemCommandHandler(
        IRetrospectivesRepository retrospectivesRepository)
    {
        _retrospectivesRepository = retrospectivesRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteRetrospectiveItemCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectivesRepository.GetByIdWithColumnsAsync(
            command.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.NotFound;
        }

        var result = retrospective.RemoveItem(command.ItemId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _retrospectivesRepository.Update(retrospective);

        return Result.Success;
    }
}