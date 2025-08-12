using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.UpdateRetrospective;

internal sealed class UpdateRetrospectiveCommandHandler
    : IRequestHandler<UpdateRetrospectiveCommand, ErrorOr<Guid>>
{
    private readonly IRetrospectiveRepository _retrospectiveRepository;

    public UpdateRetrospectiveCommandHandler(
        IRetrospectiveRepository retrospectiveRepository)
    {
        _retrospectiveRepository = retrospectiveRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        UpdateRetrospectiveCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectiveRepository.GetByIdWithColumnsAsync(
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

        _retrospectiveRepository.Update(retrospective);

        return retrospective.Id;
    }
}