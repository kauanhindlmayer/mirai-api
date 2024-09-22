using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Retrospectives.Commands.AddColumn;

public class AddColumnCommandHandler(IRetrospectivesRepository _retrospectivesRepository)
    : IRequestHandler<AddColumnCommand, ErrorOr<Retrospective>>
{
    public async Task<ErrorOr<Retrospective>> Handle(
        AddColumnCommand request,
        CancellationToken cancellationToken)
    {
        var retrospective = await _retrospectivesRepository.GetByIdAsync(
            request.RetrospectiveId,
            cancellationToken);

        if (retrospective is null)
        {
            return Error.NotFound(description: "Retrospective not found");
        }

        var column = new RetrospectiveColumn(
            title: request.Title,
            retrospectiveId: retrospective.Id);

        var addColumnResult = retrospective.AddColumn(column);
        if (addColumnResult.IsError)
        {
            return addColumnResult.Errors;
        }

        await _retrospectivesRepository.UpdateAsync(retrospective, cancellationToken);

        return retrospective;
    }
}