using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.DeleteRetrospectiveItem;

internal sealed class DeleteRetrospectiveItemCommandHandler
    : IRequestHandler<DeleteRetrospectiveItemCommand, ErrorOr<Success>>
{
    private readonly IRetrospectiveRepository _retrospectiveRepository;

    public DeleteRetrospectiveItemCommandHandler(
        IRetrospectiveRepository retrospectiveRepository)
    {
        _retrospectiveRepository = retrospectiveRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteRetrospectiveItemCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectiveRepository.GetByIdWithColumnsAsync(
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

        _retrospectiveRepository.Update(retrospective);

        return Result.Success;
    }
}