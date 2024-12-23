using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateColumn;

internal sealed class CreateColumnCommandHandler(IRetrospectivesRepository retrospectivesRepository)
    : IRequestHandler<CreateColumnCommand, ErrorOr<Retrospective>>
{
    public async Task<ErrorOr<Retrospective>> Handle(
        CreateColumnCommand command,
        CancellationToken cancellationToken)
    {
        var retrospective = await retrospectivesRepository.GetByIdWithColumnsAsync(
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

        retrospectivesRepository.Update(retrospective);

        return retrospective;
    }
}