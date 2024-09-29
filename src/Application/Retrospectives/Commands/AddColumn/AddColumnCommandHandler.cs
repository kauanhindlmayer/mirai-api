using Application.Common.Interfaces;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.AddColumn;

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
            return RetrospectiveErrors.RetrospectiveNotFound;
        }

        var column = new RetrospectiveColumn(
            title: request.Title,
            retrospectiveId: retrospective.Id);

        var addColumnResult = retrospective.AddColumn(column);
        if (addColumnResult.IsError)
        {
            return addColumnResult.Errors;
        }

        _retrospectivesRepository.Update(retrospective);

        return retrospective;
    }
}