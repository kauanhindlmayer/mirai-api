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

        var column = new RetrospectiveColumn(request.Title, retrospective.Id);
        var result = retrospective.AddColumn(column);
        if (result.IsError)
        {
            return result.Errors;
        }

        _retrospectivesRepository.Update(retrospective);

        return retrospective;
    }
}