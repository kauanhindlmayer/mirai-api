using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospectiveColumn;

internal sealed class CreateRetrospectiveColumnCommandHandler
    : IRequestHandler<CreateRetrospectiveColumnCommand, ErrorOr<Guid>>
{
    private readonly IRetrospectivesRepository _retrospectivesRepository;

    public CreateRetrospectiveColumnCommandHandler(
        IRetrospectivesRepository retrospectivesRepository)
    {
        _retrospectivesRepository = retrospectivesRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateRetrospectiveColumnCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectivesRepository.GetByIdWithColumnsAsync(
            command.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.NotFound;
        }

        var column = new RetrospectiveColumn(command.Title, retrospective.Id);

        var result = retrospective.AddColumn(column);
        if (result.IsError)
        {
            return result.Errors;
        }

        _retrospectivesRepository.Update(retrospective);

        return retrospective.Id;
    }
}