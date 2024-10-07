using Application.Common.Interfaces;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateColumn;

public class CreateColumnCommandHandler(IRetrospectivesRepository _retrospectivesRepository)
    : IRequestHandler<CreateColumnCommand, ErrorOr<Retrospective>>
{
    public async Task<ErrorOr<Retrospective>> Handle(
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

        return retrospective;
    }
}