using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.DeleteRetrospective;

internal sealed class DeleteRetrospectiveCommandHandler
    : IRequestHandler<DeleteRetrospectiveCommand, ErrorOr<Success>>
{
    private readonly IRetrospectiveRepository _retrospectiveRepository;

    public DeleteRetrospectiveCommandHandler(
        IRetrospectiveRepository retrospectiveRepository)
    {
        _retrospectiveRepository = retrospectiveRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteRetrospectiveCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectiveRepository.GetByIdAsync(
            command.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.NotFound;
        }

        _retrospectiveRepository.Remove(retrospective);

        return Result.Success;
    }
}