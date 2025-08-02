using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.DeleteRetrospective;

internal sealed class DeleteRetrospectiveCommandHandler
    : IRequestHandler<DeleteRetrospectiveCommand, ErrorOr<Success>>
{
    private readonly IRetrospectivesRepository _retrospectivesRepository;

    public DeleteRetrospectiveCommandHandler(
        IRetrospectivesRepository retrospectivesRepository)
    {
        _retrospectivesRepository = retrospectivesRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteRetrospectiveCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectivesRepository.GetByIdAsync(
            command.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.NotFound;
        }

        _retrospectivesRepository.Remove(retrospective);

        return Result.Success;
    }
}