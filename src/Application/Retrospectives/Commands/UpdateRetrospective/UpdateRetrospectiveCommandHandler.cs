using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.UpdateRetrospective;

internal sealed class UpdateRetrospectiveCommandHandler
    : IRequestHandler<UpdateRetrospectiveCommand, ErrorOr<Guid>>
{
    private readonly IRetrospectivesRepository _retrospectivesRepository;

    public UpdateRetrospectiveCommandHandler(
        IRetrospectivesRepository retrospectivesRepository)
    {
        _retrospectivesRepository = retrospectivesRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        UpdateRetrospectiveCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectivesRepository.GetByIdWithColumnsAsync(
            command.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.NotFound;
        }

        retrospective.Update(
            command.Title,
            command.MaxVotesPerUser,
            command.Template);

        _retrospectivesRepository.Update(retrospective);

        return retrospective.Id;
    }
}