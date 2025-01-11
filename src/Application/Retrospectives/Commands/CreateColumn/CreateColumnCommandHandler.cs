using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateColumn;

internal sealed class CreateColumnCommandHandler : IRequestHandler<CreateColumnCommand, ErrorOr<Guid>>
{
    private readonly IRetrospectivesRepository _retrospectivesRepository;

    public CreateColumnCommandHandler(IRetrospectivesRepository retrospectivesRepository)
    {
        _retrospectivesRepository = retrospectivesRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateColumnCommand command,
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